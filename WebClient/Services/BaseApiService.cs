using System.Net;
using System.Net.Http.Json;
using Types.Dtos;

namespace WebClient.Services;

public class BaseApiService
{
    public readonly ApiClient _client;

    public BaseApiService(ApiClient apiClient)
    {
        _client = apiClient;
    }

    public async Task<TRes?> TrySendAndRecieve<TRes>(ApiMessage message)
        where TRes: class
    {
        var res = await _client.SendApiAsync(message);
        if (!res.IsSuccessStatusCode)
        {
            var error = (await res.Content.ReadFromJsonAsync<ErrorDto>())!;
            // If res status code is 404, it not guarantee that entity not found (microservice disabled)
            // If ErrorDto StatusCode is 404, we can sure that this make a microservice
            if (error.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            throw new ApiErrorException(error);
        }
        return (await res.Content.ReadFromJsonAsync<TRes>())!;
    }
}
