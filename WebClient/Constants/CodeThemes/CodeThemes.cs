using BlazorMonaco.Editor;
using Fluxor;
using WebClient.Extensions;
using WebClient.Store.Theme;

namespace WebClient.Constants.CodeThemes;

// BlazorMonaco needs to initialized editor to define theme
// https://github.com/serdarciplak/BlazorMonaco#custom-themes
public class CodeThemes
{
    private readonly IServiceProvider _serviceProvider;
    public CodeThemes(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool Initialized { get; private set; }

    public async Task DefineCodeThemes()
    {
        if (!Initialized)
        {
            await Global.DefineTheme(DraculaTheme.ThemeToken, DraculaTheme.ThemeData);
            await Global.DefineTheme(Active4DTheme.ThemeToken, Active4DTheme.ThemeData);
            Initialized = true;
        }
    }
}
