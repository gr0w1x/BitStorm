using System.Text;
using System.Text.Json;
using Types.Dtos;

namespace WebClient.Services;

public class AuthService: BaseApiService
{
    public AuthService(ApiClient apiClient): base(apiClient) {}

    public Task<AccessRefreshTokensDto> SignIn(SignInDto dto) =>
        SendAndRecieve<AccessRefreshTokensDto>(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/sign-in", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });

    public async Task SignUp(SignUpDto dto)
    {
        await Send(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/sign-up", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });
    }

    public Task<AccessRefreshTokensDto> Confirm(ConfirmDto dto) =>
        SendAndRecieve<AccessRefreshTokensDto>(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/confirm", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });
}
