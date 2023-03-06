using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using System.Timers;
using Timer = System.Timers.Timer;

namespace WebClient.Views.Components.TerminalLabel;

public partial class TerminalLabel
{
    private string _currentText = "";
    private string _targetText = "";

    protected Timer typing = new ();

    [Parameter]
    public TimeSpan TypingInterval
    {
        get => TimeSpan.FromMilliseconds(typing.Interval);
        set => typing.Interval = value.Milliseconds;
    }

    [Parameter]
    public TimeSpan CursorBlinkInterval { get; set; }
    [Parameter]
    public string Title
    {
        get => _targetText;
        set
        {
            _targetText = value;
            _currentText = "";
            if (_targetText != _currentText && !typing.Enabled)
            {
                typing.Start();
            }
        }
    }

    protected override Task OnInitializedAsync()
    {
        typing.Elapsed += OnTyping;
        typing.Start();
        return base.OnInitializedAsync();
    }

    protected void OnTyping(object? sender, ElapsedEventArgs e)
    {
        if (_currentText != _targetText)
        {
            _currentText += _targetText[_currentText.Length];
            StateHasChanged();
        }
        else
        {
            typing.Stop();
        }
    }
}
