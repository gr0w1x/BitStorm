using System.Net.Http.Json;
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
        HttpResponseMessage response = await PostAsync("api/auth/refresh", new StringContent(_userState.Value.Tokens!.Refresh.Token));
        if (!response.IsSuccessStatusCode)
        {
            SignOut();
            throw new UnauthorizedAccessException();
        }
        var tokens = (await response.Content.ReadFromJsonAsync<AccessRefreshTokensDto>())!;
        _dispatcher.Dispatch(new SetTokensAction(tokens));
    }

    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is ApiMessage apiRequest)
        {
            if (apiRequest.NeedAuthorization && !_userState.Value.HasAccess)
            {
                await TryRefresh();
            }
            else
            {
                request.Headers.Add("Authorization", $"Bearer {_userState.Value.Tokens!.Access.Token}");
            }
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
