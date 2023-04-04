using System.Net;
using System.Net.Http.Json;
using Types.Constants.Errors;
using Types.Dtos;

namespace WebClient.Services;

public class BaseApiService
{
    public readonly ApiClient _client;

    public BaseApiService(ApiClient apiClient)
    {
        _client = apiClient;
    }

    public async Task Send(ApiMessage message)
    {
        var res = await _client.SendApiAsync(message);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadFromJsonAsync<ErrorDto>();
            throw new ApiErrorException(error ?? new ErrorDto("something happened wrong", CommonErrors.InternalServerError));
        }
    }

    public async Task<TRes> SendAndRecieve<TRes>(ApiMessage message)
        where TRes: class
    {
        var res = await _client.SendApiAsync(message);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadFromJsonAsync<ErrorDto>();
            throw new ApiErrorException(error ?? new ErrorDto("something happened wrong", CommonErrors.InternalServerError));
        }
        return (await res.Content.ReadFromJsonAsync<TRes>())!;
    }

    public async Task<TRes?> TrySendAndRecieve<TRes>(ApiMessage message)
        where TRes: class
    {
        var res = await _client.SendApiAsync(message);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadFromJsonAsync<ErrorDto>();
            if (error == null || error.Code != CommonErrors.NotFoundError)
            {
                throw new ApiErrorException(error ?? new ErrorDto("something happened wrong", CommonErrors.InternalServerError));
            }
            else
            {
                return null;
            }
        }
        return (await res.Content.ReadFromJsonAsync<TRes>())!;
    }
}
