using System.Text;
using System.Text.Json;
using Types.Dtos;

namespace WebClient.Services;

public class AuthService: BaseApiService
{
    public AuthService(ApiClient apiClient): base(apiClient) {}

    public async Task<AccessRefreshTokensDto> SignIn(SignInDto dto) =>
        (await TrySendAndRecieve<AccessRefreshTokensDto>(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/sign-in", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        }))!;

    public async Task SignUp(SignUpDto dto)
    {
        await _client.SendApiAsync(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/sign-up", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });
    }

    public async Task<AccessRefreshTokensDto> Confirm(ConfirmDto dto) =>
        (await TrySendAndRecieve<AccessRefreshTokensDto>(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/confirm", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        }))!;
}
