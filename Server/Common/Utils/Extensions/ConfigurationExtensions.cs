using Microsoft.Extensions.Configuration;
using CommonServer.Data.Types;

namespace CommonServer.Utils.Extensions;

public static class ConfigurationExtensions
{
    public static JwtOptions GetJwtOptions(this IConfiguration configuration) =>
        new ()
        {
            Secret = configuration["JWT_SECRET"]!,
            Issuer = configuration["JWT_ISSUER"]!,
            Audience = configuration["JWT_AUDIENCE"]!,
            AccessTokenDuration = JwtOptions.DefaultAccessTokenDuration,
            RefreshTokenDuration = JwtOptions.DefaultRefreshTokenDuration,
        };

    public static IConfigurationBuilder AddDbConnection(this IConfigurationBuilder builder, IConfiguration from, string connection, string prefix="MYSQL")
    {
        builder.AddInMemoryCollection(
            new Dictionary<string, string?>()
            {
                {
                    $"ConnectionStrings:{connection}",
                    String.Join(
                        ';',
                        new[]
                            {
                                ("host", from[$"{prefix}_HOST"]!),
                                ("port", from[$"{prefix}_PORT"]!),
                                ("database", from[$"{prefix}_DATABASE"]!),
                                ("user", from[$"{prefix}_USER"]!),
                                ("password", from[$"{prefix}_PASSWORD"]!)
                            }
                            .Select(p => $"{p.Item1}={p.Item2}")
                    )
                }
            }
        );

        return builder;
    }
}
