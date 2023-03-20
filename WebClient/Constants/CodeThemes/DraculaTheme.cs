using BlazorMonaco.Editor;

namespace WebClient.Constants.CodeThemes;

public static class DraculaTheme
{
    public const string ThemeToken = "dracula";

    public static readonly StandaloneThemeData ThemeData =
        new()
        {
            Base = "vs-dark",
            Inherit = true,
            Rules = new List<TokenThemeRule>
            {
                new TokenThemeRule { Background = "282a36", Token = "" },
                new TokenThemeRule { Foreground = "6272a4", Token = "comment" },
                new TokenThemeRule { Foreground = "f1fa8c", Token = "string" },
                new TokenThemeRule { Foreground = "bd93f9", Token = "constant.numeric" },
                new TokenThemeRule { Foreground = "bd93f9", Token = "constant.language" },
                new TokenThemeRule { Foreground = "bd93f9", Token = "constant.character" },
                new TokenThemeRule { Foreground = "bd93f9", Token = "constant.other" },
                new TokenThemeRule { Foreground = "ffb86c", Token = "variable.other.readwrite.instance" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "constant.character.escaped" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "constant.character.escape" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "string source" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "string source.ruby" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "keyword" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "storage" },
                new TokenThemeRule { Foreground = "8be9fd", FontStyle = "italic", Token = "storage.type" },
                new TokenThemeRule { Foreground = "50fa7b", FontStyle = "underline", Token = "entity.name.class" },
                new TokenThemeRule { Foreground = "50fa7b", FontStyle = "italic underline", Token = "entity.other.inherited-class" },
                new TokenThemeRule { Foreground = "50fa7b", Token = "entity.name.function" },
                new TokenThemeRule { Foreground = "ffb86c", FontStyle = "italic", Token = "variable.parameter" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "entity.name.tag" },
                new TokenThemeRule { Foreground = "50fa7b", Token = "entity.other.attribute-name" },
                new TokenThemeRule { Foreground = "8be9fd", Token = "support.function" },
                new TokenThemeRule { Foreground = "6be5fd", Token = "support.constant" },
                new TokenThemeRule { Foreground = "66d9ef", FontStyle = " italic", Token = "support.type" },
                new TokenThemeRule { Foreground = "66d9ef", FontStyle = " italic", Token = "support.class" },
                new TokenThemeRule { Foreground = "f8f8f0", Background = "ff79c6", Token = "invalid" },
                new TokenThemeRule { Foreground = "f8f8f0", Background = "bd93f9", Token = "invalid.deprecated" },
                new TokenThemeRule { Foreground = "cfcfc2", Token = "meta.structure.dictionary.json string.quoted.double.json" },
                new TokenThemeRule { Foreground = "6272a4", Token = "meta.diff" },
                new TokenThemeRule { Foreground = "6272a4", Token = "meta.diff.header" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "markup.deleted" },
                new TokenThemeRule { Foreground = "50fa7b", Token = "markup.inserted" },
                new TokenThemeRule { Foreground = "e6db74", Token = "markup.changed" },
                new TokenThemeRule { Foreground = "bd93f9", Token = "constant.numeric.line-number.find-in-files - match" },
                new TokenThemeRule { Foreground = "e6db74", Token = "entity.name.filename" },
                new TokenThemeRule { Foreground = "f83333", Token = "message.error" },
                new TokenThemeRule { Foreground = "eeeeee", Token = "punctuation.definition.string.begin.json - meta.structure.dictionary.value.json" },
                new TokenThemeRule { Foreground = "eeeeee", Token = "punctuation.definition.string.end.json - meta.structure.dictionary.value.json" },
                new TokenThemeRule { Foreground = "8be9fd", Token = "meta.structure.dictionary.json string.quoted.double.json" },
                new TokenThemeRule { Foreground = "f1fa8c", Token = "meta.structure.dictionary.value.json string.quoted.double.json" },
                new TokenThemeRule { Foreground = "50fa7b", Token = "meta meta meta meta meta meta meta.structure.dictionary.value string" },
                new TokenThemeRule { Foreground = "ffb86c", Token = "meta meta meta meta meta meta.structure.dictionary.value string" },
                new TokenThemeRule { Foreground = "ff79c6", Token = "meta meta meta meta meta.structure.dictionary.value string" },
                new TokenThemeRule { Foreground = "bd93f9", Token = "meta meta meta meta.structure.dictionary.value string" },
                new TokenThemeRule { Foreground = "50fa7b", Token = "meta meta meta.structure.dictionary.value string" },
                new TokenThemeRule { Foreground = "ffb86c", Token = "meta meta.structure.dictionary.value string" },
            },
            Colors = new Dictionary<string, string>
            {
                ["editor.foreground"] = "#f8f8f2",
                ["editor.background"] = "#282a36",
                ["editor.selectionBackground"] = "#44475a",
                ["editor.lineHighlightBackground"] = "#44475a",
                ["editorCursor.foreground"] = "#f8f8f0",
                ["editorWhitespace.foreground"] = "#3B3A32",
                ["editorIndentGuide.activeBackground"] = "#9D550FB0",
                ["editor.selectionHighlightBorder"] = "#222218",
            }
        };
}
