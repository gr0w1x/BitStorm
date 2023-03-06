using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Types.Entities;

namespace Gateway.Services;

public class JwtBarerChecksUsersHandler: JwtBearerHandler
{
    private readonly OnlyUsersContext _usersContext;

    public JwtBarerChecksUsersHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        OnlyUsersContext usersContext
    ) : base(options, logger, encoder, clock)
    {
        _usersContext = usersContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();

        if (!result.Succeeded)
        {
            return result;
        }

        Guid id = Guid.Parse(result.Principal.Claims.First<Claim>(claim => claim.Type == "id").Value);
        UserRoles roles = (UserRoles)Convert.ToInt32(result.Principal.Claims.First<Claim>(claim => claim.Type == ClaimTypes.Role).Value);

        var user = await _usersContext.Users.FindAsync(id);

        if (user == null || user.Roles != roles)
        {
            return AuthenticateResult.Fail("Jwt token content is not correct");
        }

        return result;
    }
}
