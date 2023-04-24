using Types.Entities;

namespace Types.Dtos;

public record UserClaims(Guid UserId, UserRoles Roles, int Trophies)
{
    public static explicit operator UserClaims? (PublicUser? user)
        => user != null
            ? new UserClaims(
                user.Id,
                user.Roles,
                user.Trophies
            )
            : null;
}

public record JwtToken(string Token, DateTime Expires);

public record AccessRefreshTokensDto(Guid UserId, JwtToken Access, JwtToken Refresh);
