using Types.Entities;

namespace WebClient.Services;

public class UsersService: BaseApiService
{
    public UsersService(ApiClient apiClient): base(apiClient) { }

    public async Task<PublicUser?> GetUser(Guid userId) =>
        await TrySendAndRecieve<PublicUser>(new ApiMessage()
        {
            RequestUri = new Uri($"/api/users/{userId}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
        });
}
