using Microsoft.AspNetCore.Components;

namespace WebClient.Views.Components.MarkdownEditor;

public partial class MarkdownEditor
{
    PSC.Blazor.Components.MarkdownEditor.MarkdownEditor? _editor;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; }

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
                _editor?.SetValueAsync(value);
            }
        }
    }
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    public async Task OnValueChanged(string value)
    {
        _value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
