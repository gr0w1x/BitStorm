using Fluxor;

namespace WebClient.Store.Theme;

public static class ThemeReducers
{
    [ReducerMethod]
    public static ThemeState SetStateReducer(ThemeState state, SetThemeAction action) =>
        state with
        {
            Theme = action.Theme
        };

    [ReducerMethod(typeof(ToggleThemeAction))]
    public static ThemeState ToggleThemeReducer(ThemeState state) =>
        state with
        {
            Theme = state.Theme == Theme.Light ? Theme.Dark : Theme.Light
        };
}
