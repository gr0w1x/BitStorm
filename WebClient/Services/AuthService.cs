using System.Text;
using System.Text.Json;
using Types.Dtos;

namespace WebClient.Services;

public class AuthService
{
    private readonly ApiClient _client;

    public AuthService(ApiClient apiClient)
    {
        _client = apiClient;
    }

    public Task<HttpResponseMessage> SignIn(SignInDto dto) =>
        _client.SendAsync(new ApiMessage()
        {
            RequestUri = new Uri("/api/users/auth/sign-in", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });

    public Task<HttpResponseMessage> SignUp(SignUpDto dto) =>
        _client.SendAsync(new ApiMessage()
        {
            RequestUri = new Uri("/api/users/auth/sign-up", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
        });
}
