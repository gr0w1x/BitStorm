using System.Security.Claims;
using Types.Dtos;
using Types.Entities;

namespace CommonServer.Utils.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UserClaims? GetUserClaims(this ClaimsPrincipal principal)
    {
        if (
            Guid.TryParse(
                principal.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value,
                out Guid id
            ) && int.TryParse(
                principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value,
                out int roles
            ) && int.TryParse(
                principal.Claims.FirstOrDefault(claim => claim.Type == "trophies")?.Value,
                out int trophies
            )
        )
        {
            return new UserClaims(id, (UserRoles)roles, trophies);
        }
        return null;
    }
}
