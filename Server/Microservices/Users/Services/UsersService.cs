using System.Net;
using Types.Constants.Errors;
using Types.Dtos;
using Users.Repositories;

namespace Users.Services;

public class UsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<IResult> GetUser(Guid guid)
    {
        var user = await _usersRepository.GetById(guid);
        if (user == null)
        {
            return Results.NotFound(new ErrorDto("user not found", CommonErrors.NotFoundError));
        }
        return Results.Json(user);
    }
}
