using Fluxor;

namespace WebClient.Store.Theme;

public enum Theme
{
    Light,
    Dark
}

[FeatureState]
public record ThemeState(Theme Theme)
{
    public ThemeState(): this(Theme.Dark) {}
}
