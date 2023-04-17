using System.Text;
using System.Text.Json;
using Types.Dtos;

namespace WebClient.Services;

public class AuthService
{
    private readonly ApiClient _apiClient;

    public AuthService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<AccessRefreshTokensDto> SignIn(SignInDto dto) =>
        _apiClient.SendAndRecieve<AccessRefreshTokensDto>(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/sign-in", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });

    public async Task SignUp(SignUpDto dto)
    {
        await _apiClient.Send(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/sign-up", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });
    }

    public Task<AccessRefreshTokensDto> Confirm(ConfirmDto dto) =>
        _apiClient.SendAndRecieve<AccessRefreshTokensDto>(new ApiMessage()
        {
            RequestUri = new Uri("/api/auth/confirm", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });
}
