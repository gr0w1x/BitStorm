using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;
using WebClient.Constants.CodeThemes;
using WebClient.Extensions;

namespace WebClient.Views.Components.CodeEditor;

public partial class CodeEditor
{
    private StandaloneCodeEditor? _editor;

    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor _)
    {
        return new StandaloneEditorConstructionOptions
        {
            Value = Value,
            Language = Language,
            AutomaticLayout = true,
            TabSize = 2,
        };
    }

    private string _language = "plaintext";
    [Parameter]
    public string Language
    {
        get => _language;
        set
        {
            if (_language != value)
            {
                _language = value;
                if (_editor != null)
                {
                    SetLanguage(value);
                }
            }
        }
    }

    private string _value = "";
    [Parameter]
    public string Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                _editor?.SetValue(value);
            }
        }
    }
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; }

    [Inject]
    public CodeThemes CodeThemes { get; set; }

    protected async Task SetLanguage(string language)
    {
        await Global.SetModelLanguage(await _editor!.GetModel(), language);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !CodeThemes.Initialized)
        {
            await CodeThemes.DefineCodeThemes();
            await Global.SetTheme(ComponentState.Value.Theme.ToCodeTheme());
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected async Task OnValueChanged(ModelContentChangedEvent _)
    {
        var value = await _editor!.GetValue();
        _value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
