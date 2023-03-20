using Blazored.LocalStorage;
using BlazorMonaco.Editor;
using Fluxor;
using Microsoft.JSInterop;
using WebClient.Constants.CodeThemes;
using WebClient.Extensions;

namespace WebClient.Store.Theme;

public class ThemeMiddleware: Middleware
{
    public const string TokensKey = "theme";

    private readonly ISyncLocalStorageService _localStorage;

    private readonly IState<ThemeState> _themeState;

    private readonly IJSRuntime _jsRuntime;

    public ThemeMiddleware(
        ISyncLocalStorageService localStorage,
        IState<ThemeState> themeState,
        IJSRuntime jsRuntime
    )
    {
        _localStorage = localStorage;
        _themeState = themeState;
        _jsRuntime = jsRuntime;
    }

    public override Task InitializeAsync(IDispatcher dispatcher, IStore store)
    {
        if (_localStorage.ContainKey(TokensKey))
        {
            try
            {
                var theme = (Theme) Enum.Parse(typeof(Theme), _localStorage.GetItem<string>(TokensKey), true);
                if (theme != _themeState.Value.Theme)
                {
                    dispatcher.Dispatch(new SetThemeAction(theme));
                }
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
        if (action is SetThemeAction or ToggleThemeAction)
        {
            _localStorage.SetItemAsString(TokensKey, _themeState.Value.Theme.ToString());
            Global.SetTheme(_themeState.Value.Theme.ToCodeTheme());
            _jsRuntime.InvokeVoidAsync("setTheme", _themeState.Value.Theme.ToString());
        }
        base.AfterDispatch(action);
    }
}
