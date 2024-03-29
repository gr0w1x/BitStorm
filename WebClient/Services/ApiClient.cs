using System.Net.Http.Json;
using System.Text;
using Fluxor;
using Types.Constants.Errors;
using Types.Dtos;
using WebClient.Store.User;

namespace WebClient.Services;

public class ApiMessage: HttpRequestMessage
{
    // If user signed in, ApiClient refreshs tokens when needed. No authorization required
    public bool OptionalAuthorization { get; set; }
    // Authorization required
    public bool NeedAuthorization { get; set; }
}

public class ApiClient: HttpClient
{
    public readonly IState<UserState> UserState;
    private readonly IDispatcher _dispatcher;

    public ApiClient(IState<UserState> userState, IDispatcher dispatcher)
    {
        UserState = userState;
        _dispatcher = dispatcher;
    }

    private void SignOut()
    {
        _dispatcher.Dispatch(new SignOutAction());
    }

    public async Task TryRefresh()
    {
        if (!UserState.Value.CanRefresh)
        {
            SignOut();
            throw new UnauthorizedAccessException();
        }
        HttpResponseMessage response = await PostAsync(
            "api/auth/refresh",
            new StringContent($"\"{UserState.Value.Tokens!.Refresh.Token}\"", Encoding.UTF8, "application/json")
        );
        if (!response.IsSuccessStatusCode)
        {
            SignOut();
            throw new UnauthorizedAccessException();
        }
        var tokens = (await response.Content.ReadFromJsonAsync<AccessRefreshTokensDto>())!;
        _dispatcher.Dispatch(new SetTokensAction(tokens));
    }

    public async Task<HttpResponseMessage> SendApiAsync(HttpRequestMessage request)
    {
        if (request is ApiMessage apiRequest)
        {
            if (
                (apiRequest.NeedAuthorization && !UserState.Value.HasAccess) ||
                (apiRequest.OptionalAuthorization && UserState.Value?.Tokens != null && !UserState.Value.HasAccess)
            )
            {
                await TryRefresh();
            }
            if (UserState.Value?.Tokens != null)
            {
                request.Headers.Add("Authorization", $"Bearer {UserState.Value.Tokens!.Access.Token}");
            }
        }
        return await SendAsync(request);
    }

    public async Task Send(ApiMessage message)
    {
        var res = await SendApiAsync(message);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadFromJsonAsync<ErrorDto>();
            throw new ApiErrorException(error ?? new ErrorDto("something happened wrong", CommonErrors.InternalServerError));
        }
    }

    public async Task<TRes> SendAndRecieve<TRes>(ApiMessage message)
        where TRes: class
    {
        var res = await SendApiAsync(message);
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
        var res = await SendApiAsync(message);
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
