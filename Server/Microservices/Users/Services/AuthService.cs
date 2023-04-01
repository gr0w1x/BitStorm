using System.Text.Json;
using System.Net;
using CommonServer.Asp.HostedServices;
using CommonServer.Data.Messages;
using CommonServer.Data.Repositories;
using CommonServer.Data.Models;
using Types.Entities;
using Users.Models;
using Users.Repositories;
using Users.Templates;
using Encoding = System.Text.Encoding;
using Types.Dtos;
using Microsoft.AspNetCore.WebUtilities;

namespace Users.Services;

public class AuthService
{
    private readonly JwtService _jwtService;
    private readonly IHasher _hasher;
    private readonly IUnitOfWork _work;

    private readonly IUsersRepository _usersRepository;
    private readonly IRefreshTokenRecordsRepository _refreshTokenRecordsRepository;
    private readonly IConfirmRecordsRepository _confirmRecordsRepository;

    private readonly RabbitMqProvider _rabbitMqProvider;
    private readonly IConfiguration _configuration;

    public AuthService(
        JwtService jwtService,
        IHasher hasher,
        IUnitOfWork work,
        IUsersRepository usersRepository,
        IRefreshTokenRecordsRepository refreshTokenRecordsRepository,
        IConfirmRecordsRepository confirmRepository,
        RabbitMqProvider rabbitMqProvider,
        IConfiguration configuration
    )
    {
        _jwtService = jwtService;
        _hasher = hasher;
        _work = work;
        _usersRepository = usersRepository;
        _refreshTokenRecordsRepository = refreshTokenRecordsRepository;
        _confirmRecordsRepository = confirmRepository;
        _rabbitMqProvider = rabbitMqProvider;
        _configuration = configuration;
    }

    private static (bool CanAccess, IResult ReasonWhyNot) CanGivenAccess(User user)
    {
        if (!user.Confirmed)
        {
            return (false, Results.BadRequest(new ErrorDto("User is not confirmed", HttpStatusCode.BadRequest)));
        }

        // TODO: add banned check

        return (true, Results.Ok());
    }

    private async Task<IResult> TryCreateAndSaveTokens(User user)
    {
        var (canAccess, reason) = CanGivenAccess(user);

        if (!canAccess)
        {
            return reason;
        }

        var tokens = _jwtService.CreateAccessRefreshTokens(user);

        await _refreshTokenRecordsRepository.Create(
            new RefreshTokenRecord(tokens.Refresh.Token, user.Id, tokens.Refresh.Expires)
        );

        await _work.Save();

        return Results.Json(tokens);
    }

    public async Task<IResult> SignIn(string emailOrUsername, string password)
    {
        // Username cannot include '@' character, no complex validation needed here
        User? user = await _usersRepository.GetByEmailOrUsername(emailOrUsername, emailOrUsername);

        if (user == null || !_hasher.Compare(password, user.Password))
        {
            return Results.NotFound(new ErrorDto("Invalid email or password", HttpStatusCode.NotFound));
        }

        return await TryCreateAndSaveTokens(user);
    }

    public async Task<IResult> SignUp(string email, string username, string password)
    {
        User? user = await _usersRepository.GetByEmailOrUsername(email, username);

        if (user != null)
        {
            return Results.BadRequest(new ErrorDto($"User with {(
                user.Username == username
                    ? $"username {user.Username}"
                    : $"email {user.Email}"
            )} already exists", HttpStatusCode.BadRequest));
        }

        if (_rabbitMqProvider.Model == null)
        {
            return Results.StatusCode(500);
        }

        var basicProperties = _rabbitMqProvider.Model.CreateBasicProperties();
        basicProperties.CorrelationId = email;

        User signed = new ()
        {
            Username = username,
            Email = email,
            Password = _hasher.Hash(password),
            Roles = UserRoles.Common,
            Confirmed = false,
            Trophies = 0,
            Registered = DateTimeOffset.Now,
            LastSeen = DateTimeOffset.Now
        };

        var confirmLink = await GenerateConfirmLink(signed.Email);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
            new MailMessage(
                "Email confirmation",
                UsersMailTemplate.ConfirmEmail(new ConfirmEmail(signed, confirmLink)),
                null,
                new[] { email }
            )
        ));

        _rabbitMqProvider.Model.BasicPublish(
            exchange: "",
            routingKey: _configuration["MAILER_REQUEST_QUEUE"]!,
            mandatory: false,
            basicProperties: basicProperties,
            body: body
        );

        await _usersRepository.Create(signed);
        await _work.Save();

        return Results.Ok();
    }

    public async Task<IResult> Confirm(Guid confirmId)
    {
        var confirm = await _confirmRecordsRepository.GetById(confirmId);

        if (confirm == null)
        {
            return Results.BadRequest(new ErrorDto("Wrong confirm code", HttpStatusCode.BadRequest));
        }

        User? user = await _usersRepository.GetByEmailOrUsername(confirm.Email, String.Empty);

        if (user == null || (DateTimeOffset.UtcNow - user.Registered) > UserConstants.ConfirmPeriod)
        {
            return Results.BadRequest(new ErrorDto("Account confirmation expired", HttpStatusCode.BadRequest));
        }

        if (user.Confirmed)
        {
            return Results.BadRequest(new ErrorDto("Already confirmed", HttpStatusCode.BadRequest));
        }

        user.Confirmed = true;

        await _usersRepository.Update(user);
        await _confirmRecordsRepository.Delete(confirm);
        await _work.Save();

        return await TryCreateAndSaveTokens(user);
    }

    public async Task<IResult> Refresh(string refreshToken)
    {
        RefreshTokenRecord? refreshTokenRecord = await _refreshTokenRecordsRepository.GetById(refreshToken);

        if (refreshTokenRecord == null || refreshTokenRecord.Expired < DateTimeOffset.UtcNow)
        {
            return Results.NotFound(new ErrorDto("Refresh token invalid or expired", HttpStatusCode.NotFound));
        }

        User? user = await _usersRepository.GetById(refreshTokenRecord.UserId);

        if (user == null)
        {
            return Results.NotFound(new ErrorDto("User was deleted", HttpStatusCode.NotFound));
        }

        await _refreshTokenRecordsRepository.DeleteExpired();
        await _work.Save();

        return await TryCreateAndSaveTokens(user);
    }

    public async Task<string> GenerateConfirmLink(string email)
    {
        var confirmRecord = new ConfirmRecord(email, DateTimeOffset.UtcNow + UserConstants.ConfirmPeriod);

        await _confirmRecordsRepository.Create(confirmRecord);
        await _work.Save();

        var query = new Dictionary<string, string?>()
        {
            { "confirm", confirmRecord.Id.ToString() }
        };

        return QueryHelpers.AddQueryString($"{_configuration["WEB_APPLICATION_URL"]!}/confirm", query);
    }

    public async Task DeleteExpired()
    {
        await _refreshTokenRecordsRepository.DeleteExpired();
        await _confirmRecordsRepository.DeleteExpired();
        await _work.Save();
    }
}
