using Types.Entities;

namespace Types.Dtos;

public record AccessJwtTokenContent(Guid UserId, UserRoles Roles, int Trophies);

public record JwtToken(string Token, DateTime Expires);

public record AccessRefreshTokensDto(Guid UserId, JwtToken Access, JwtToken Refresh);
