using WebClient.Constants.CodeThemes;
using WebClient.Store.Theme;

namespace WebClient.Extensions;

public static class ThemeExtensions
{
    public static string ToCodeTheme(this Theme theme)
    {
        switch(theme)
        {
            case Theme.Light:
            {
                return Active4DTheme.ThemeToken;
            }
            case Theme.Dark:
            {
                return DraculaTheme.ThemeToken;
            }
        }
        throw new Exception($"No provided code theme: {theme}");
    }
}
