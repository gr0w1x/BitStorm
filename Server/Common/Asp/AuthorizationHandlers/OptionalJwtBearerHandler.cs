using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CommonServer.Asp.AuthorizationHandlers;

public class OptionalJwtBearerHandler: JwtBearerAdsToContextHandler
{
    public const string SchemeName = "Optional Bearer";

    public OptionalJwtBearerHandler (
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock) {}

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();
        if (result.Succeeded)
        {
            return result;
        }
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), SchemeName));
    }
}
