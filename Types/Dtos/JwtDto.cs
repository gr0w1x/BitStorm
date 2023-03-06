namespace Types.Dtos;

public record JwtToken(string Token, DateTime Expires);

public record AccessRefreshTokensDto(JwtToken Access, JwtToken Refresh);
