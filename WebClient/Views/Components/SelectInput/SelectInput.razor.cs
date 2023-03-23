using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace WebClient.Views.Components.SelectInput;

public record SelectItem<TItem> (TItem? Item, bool Selected);

public partial class SelectInput<TItem>: InputSelect<TItem>
{
    [Parameter]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public RenderFragment<SelectItem<TItem>> RenderItem { get; set; } =
        (context) =>
            (RenderTreeBuilder builder) =>
                builder.AddContent(0, context.Item.ToString());

    [Parameter]
    public RenderFragment<SelectItem<TItem>>? RenderSelected { get; set; }

    [Parameter]
    public EventCallback<TItem> OnChange { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    private async Task OnSelect(TItem item)
    {
        CurrentValue = item;
        Value = item;
        await ValueChanged.InvokeAsync(item);
        await OnChange.InvokeAsync(item);
    }
}
