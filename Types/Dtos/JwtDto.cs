using Types.Entities;

namespace Types.Dtos;

public record AccessJwtTokenContent(Guid UserId, UserRoles Roles, int Trophies)
{
    public static explicit operator PublicUser? (AccessJwtTokenContent? jwt)
        => jwt != null
            ? new PublicUser()
            {
                Id = jwt.UserId,
                Roles = jwt.Roles,
                Trophies = jwt.Trophies
            }
            : null;
}

public record JwtToken(string Token, DateTime Expires);

public record AccessRefreshTokensDto(Guid UserId, JwtToken Access, JwtToken Refresh);
