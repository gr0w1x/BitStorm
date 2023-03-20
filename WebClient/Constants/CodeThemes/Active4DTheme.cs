using BlazorMonaco.Editor;

namespace WebClient.Constants.CodeThemes;

public static class Active4DTheme
{
    public const string ThemeToken = "active4d";

    public static readonly StandaloneThemeData ThemeData =
        new()
        {
            Base = "vs",
            Inherit = true,
            Rules = new List<TokenThemeRule>
            {
                new TokenThemeRule { Background = "FFFFFF", Token = "" },
                new TokenThemeRule { Background = "e2e9ff5e", Token = "text.html source.active4d" },
                new TokenThemeRule { Foreground = "000000", Token = "text.xml" },
                new TokenThemeRule { Foreground = "af82d4", Token = "comment.line" },
                new TokenThemeRule { Foreground = "af82d4", Token = "comment.block" },
                new TokenThemeRule { Foreground = "666666", Token = "string" },
                new TokenThemeRule { Foreground = "66ccff", FontStyle = "bold", Token = "string.interpolated variable" },
                new TokenThemeRule { Foreground = "a8017e", Token = "constant.numeric" },
                new TokenThemeRule { Foreground = "66ccff", FontStyle = "bold", Token = "constant.other.date" },
                new TokenThemeRule { Foreground = "66ccff", FontStyle = "bold", Token = "constant.other.time" },
                new TokenThemeRule { Foreground = "a535ae", Token = "constant.language" },
                new TokenThemeRule { Foreground = "6392ff", FontStyle = "bold", Token = "variable.other.local" },
                new TokenThemeRule { Foreground = "0053ff", FontStyle = "bold", Token = "variable" },
                new TokenThemeRule { Foreground = "6988ae", Token = "variable.other.table-field" },
                new TokenThemeRule { Foreground = "006699", FontStyle = "bold", Token = "keyword" },
                new TokenThemeRule { Foreground = "ff5600", Token = "storage" },
                new TokenThemeRule { Foreground = "21439c", Token = "entity.name.type" },
                new TokenThemeRule { Foreground = "21439c", Token = "entity.name.function" },
                new TokenThemeRule { Foreground = "7a7a7a", Token = "meta.tag" },
                new TokenThemeRule { Foreground = "016cff", Token = "entity.name.tag" },
                new TokenThemeRule { Foreground = "963dff", Token = "entity.other.attribute-name" },
                new TokenThemeRule { Foreground = "45ae34", FontStyle = "bold", Token = "support.function" },
                new TokenThemeRule { Foreground = "b7734c", Token = "support.constant" },
                new TokenThemeRule { Foreground = "a535ae", Token = "support.type" },
                new TokenThemeRule { Foreground = "a535ae", Token = "support.class" },
                new TokenThemeRule { Foreground = "a535ae", Token = "support.variable" },
                new TokenThemeRule { Foreground = "ffffff", Background = "990000", Token = "invalid" },
                new TokenThemeRule { Foreground = "ffffff", Background = "656565", Token = "meta.diff" },
                new TokenThemeRule { Foreground = "ffffff", Background = "1b63ff", Token = "meta.diff.range" },
                new TokenThemeRule { Foreground = "000000", Background = "ff7880", Token = "markup.deleted.diff" },
                new TokenThemeRule { Foreground = "000000", Background = "98ff9a", Token = "markup.inserted.diff" },
                new TokenThemeRule { Foreground = "5e5e5e", Token = "source.diff" },
            },
            Colors = new Dictionary<string, string>
            {
                ["editor.foreground"] = "#3B3B3B",
                ["editor.background"] = "#FFFFFF",
                ["editor.selectionBackground"] = "#BAD6FD",
                ["editor.lineHighlightBackground"] = "#00000012",
                ["editorCursor.foreground"] = "#000000",
                ["editorWhitespace.foreground"] = "#BFBFBF",
            }
        };
}
