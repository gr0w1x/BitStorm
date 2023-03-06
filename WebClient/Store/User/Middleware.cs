using Blazored.LocalStorage;
using Fluxor;
using Types.Dtos;

namespace WebClient.Store.User;

public class UserMiddleware: Middleware
{
    public const string TokensKey = "tokens";

    private readonly ISyncLocalStorageService _localStorage;

    public UserMiddleware(ISyncLocalStorageService localStorageService)
    {
       _localStorage = localStorageService;
    }

    public override Task InitializeAsync(IDispatcher dispatcher, IStore store)
    {
        if (_localStorage.ContainKey(TokensKey))
        {
            try
            {
                var tokens = _localStorage.GetItem<AccessRefreshTokensDto>(TokensKey);
                dispatcher.Dispatch(new SetTokensAction(tokens));
            }
            catch
            {
                _localStorage.RemoveItem(TokensKey);
            }
        }
        return base.InitializeAsync(dispatcher, store);
    }

    public override void AfterDispatch(object action)
    {
        if (action is SetTokensAction tokensAction)
        {
            _localStorage.SetItem(TokensKey, tokensAction.Tokens);
        }
        base.AfterDispatch(action);
    }
}
