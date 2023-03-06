using Microsoft.AspNetCore.Components;

namespace WebClient.Views.Components.Button;

public partial class Button
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> InputAttributes { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }
}
