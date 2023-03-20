using System.Net.Http.Json;
using System.Text;
using Fluxor;
using Types.Dtos;
using WebClient.Store.User;

namespace WebClient.Services;

public class ApiMessage: HttpRequestMessage
{
    public bool NeedAuthorization { get; set; }
}

public class ApiClient: HttpClient
{
    private readonly IState<UserState> _userState;
    private readonly IDispatcher _dispatcher;

    public ApiClient(IState<UserState> userState, IDispatcher dispatcher)
    {
        _userState = userState;
        _dispatcher = dispatcher;
    }

    private void SignOut()
    {
        _dispatcher.Dispatch(new SignOutAction());
    }

    public async Task TryRefresh()
    {
        if (!_userState.Value.CanRefresh)
        {
            SignOut();
            throw new UnauthorizedAccessException();
        }
        HttpResponseMessage response = await PostAsync(
            "api/auth/refresh",
            new StringContent($"\"{_userState.Value.Tokens!.Refresh.Token}\"", Encoding.UTF8, "application/json")
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
            if (apiRequest.NeedAuthorization && !_userState.Value.HasAccess)
            {
                await TryRefresh();
            }
            if (_userState.Value.Tokens != null)
            {
                request.Headers.Add("Authorization", $"Bearer {_userState.Value.Tokens!.Access.Token}");
            }
        }
        return await SendAsync(request);
    }
}
