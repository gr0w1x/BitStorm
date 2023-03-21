using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebClient.Views.Components.SelectInput;

public record SelectItem<TItem> (TItem? Item, bool Selected);

public partial class SelectInput<TItem>: InputSelect<TItem>
    where TItem: IComparable
{
    [Parameter]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public RenderFragment<SelectItem<TItem>> RenderItem { get; set; }

    [Parameter]
    public RenderFragment<SelectItem<TItem>> RenderSelected { get; set; }

    [Parameter]
    public EventCallback<TItem> OnChange { get; set; }

    private async Task OnSelect(TItem item)
    {
        CurrentValue = item;
        Value = item;
        await ValueChanged.InvokeAsync(item);
        await OnChange.InvokeAsync(item);
    }
}
