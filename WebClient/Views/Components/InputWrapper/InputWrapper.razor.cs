using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace WebClient.Views.Components.InputWrapper;

public partial class InputWrapper<T>
{
    [Parameter]
    public Expression<Func<T>>? For { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public bool WithValidation { get; set; } = true;

    [Parameter]
    public RenderFragment ChildContent { get; set; }
}
