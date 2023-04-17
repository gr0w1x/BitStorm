using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Types.Dtos;
using Types.Entities;

namespace CommonServer.Asp.AuthorizationHandlers;

public class JwtBearerAdsToContextHandler: JwtBearerHandler
{
    public const string UserItem = "user";

    public JwtBearerAdsToContextHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock) {}

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();

        if (!result.Succeeded)
        {
            return result;
        }

        try
        {
            Guid id = Guid.Parse(result.Principal.Claims.First(claim => claim.Type == "id").Value);
            UserRoles roles = (UserRoles)Convert.ToInt32(result.Principal.Claims.First(claim => claim.Type == ClaimTypes.Role).Value);
            int trophies = Convert.ToInt32(result.Principal.Claims.First(claim => claim.Type == "trophies").Value);

            Context.Items[UserItem] = new AccessJwtTokenContent(id, roles, trophies);

            return result;
        }
        catch (Exception e)
        {
            return AuthenticateResult.Fail(e);
        }
    }
}
