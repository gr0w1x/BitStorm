using Types.Entities;

namespace WebClient.Services;

public class UsersService: BaseCachedService<Guid, PublicUser?>
{
    private readonly ApiClient _apiClient;

    public UsersService(ApiClient apiClient): base()
    {
        _apiClient = apiClient;
    }

    protected override Task<PublicUser?> Load(Guid userId) =>
        _apiClient.TrySendAndRecieve<PublicUser>(new ApiMessage()
        {
            RequestUri = new Uri($"/api/users/{userId}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
        });
}
