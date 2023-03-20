using System.Net;
using Blazored.LocalStorage;
using Fluxor;
using Types.Dtos;
using Types.Entities;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.UserMenu;
using WebClient.Typing;

namespace WebClient.Store.User;

public class UserMiddleware: Middleware
{
    public const string TokensKey = "tokens";

    private readonly ISyncLocalStorageService _localStorage;
    private readonly IState<UserState> _userState;
    private readonly UsersService _usersService;

    public UserMiddleware(
        ISyncLocalStorageService localStorageService,
        UsersService usersService,
        IState<UserState> userState
    )
    {
       _localStorage = localStorageService;
       _usersService = usersService;
       _userState = userState;
    }

    private IDispatcher _dispatcher;

    public override async Task InitializeAsync(IDispatcher dispatcher, IStore store)
    {
        _dispatcher = dispatcher;
        if (_localStorage.ContainKey(TokensKey))
        {
            try
            {
                var tokens = _localStorage.GetItem<AccessRefreshTokensDto>(TokensKey);
                dispatcher.Dispatch(new SetTokensAction(tokens));
                await base.InitializeAsync(dispatcher, store);
                return;
            }
            catch
            {
                _localStorage.RemoveItem(TokensKey);
            }
        }
        dispatcher.Dispatch(new InitiateAction());
        await base.InitializeAsync(dispatcher, store);
    }

    public override bool MayDispatchAction(object action)
    {
        if (action is LoadUserAction loadUserAction)
        {
            LoadUser(loadUserAction.UserId);
            return false;
        }
        return true;
    }

    public async Task LoadUser(Guid userId)
    {
        _dispatcher.Dispatch(new SetUxState<UserMenuState>(UxState.Loading));
        try
        {
            PublicUser user = (await _usersService.GetUser(userId))!;
            _dispatcher.Dispatch(new SetUserAction(user));
            _dispatcher.Dispatch(new SetUxState<UserMenuState>(UxState.Success));
        }
        catch (Exception e)
        {
            if (
                e is NullReferenceException ||
                (e is ApiErrorException apiError && apiError.Error.StatusCode == HttpStatusCode.NotFound)
            )
            {
                _dispatcher.Dispatch(new SignOutAction());
                _dispatcher.Dispatch(new SetUxState<UserMenuState>(UxState.Success));
                return;
            }
            _dispatcher.Dispatch(new SetUxState<UserMenuState>(UxState.Error));
        }
    }

    public override void AfterDispatch(object action)
    {
        if (action is SetTokensAction tokensAction)
        {
            _localStorage.SetItem(TokensKey, tokensAction.Tokens);
            if (_userState.Value.User?.Id != tokensAction.Tokens.UserId)
            {
                _dispatcher.Dispatch(new LoadUserAction(tokensAction.Tokens.UserId));
            }
        }
        else if (action is SignOutAction)
        {
            _localStorage.RemoveItem(TokensKey);
        }
        base.AfterDispatch(action);
    }
}
