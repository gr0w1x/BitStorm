using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CommonServer.Data.Models;
using CommonServer.Data.Types;
using Microsoft.IdentityModel.Tokens;
using Types.Dtos;

namespace Users.Services;

public class JwtService
{
    private readonly JwtOptions _options;

    public JwtService(JwtOptions options)
    {
        _options = options;
    }

    private JwtToken CreateToken(IEnumerable<Claim> claims, DateTime expires) =>
        new (
            Token: new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(_options.Key, SecurityAlgorithms.HmacSha256)

                )
            ),
            Expires: expires
        );

    private JwtToken CreateToken(IEnumerable<Claim> claims, TimeSpan duration) =>
        CreateToken(claims, DateTime.UtcNow.Add(duration));

    public AccessRefreshTokensDto CreateAccessRefreshTokens(User user) =>
        new (
            UserId: user.Id,
            Access: CreateToken(
                new []
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("roles", ((int)user.Roles).ToString()),
                    new Claim("trophies", user.Trophies.ToString())
                },
                _options.AccessTokenDuration
            ),
            Refresh: CreateToken(
                new []
                {
                    new Claim("id", user.Id.ToString())
                },
                _options.RefreshTokenDuration
            )
        );
}
