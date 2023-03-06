using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CommonServer.Data.Types;

public class JwtOptions
{
    public string Secret
    {
        set => Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(value));
    }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public TimeSpan AccessTokenDuration { get; set; }
    public TimeSpan RefreshTokenDuration { get; set; }
    public SymmetricSecurityKey Key { get; private set; }

    public static readonly TimeSpan DefaultAccessTokenDuration = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan DefaultRefreshTokenDuration = TimeSpan.FromDays(7);
}
