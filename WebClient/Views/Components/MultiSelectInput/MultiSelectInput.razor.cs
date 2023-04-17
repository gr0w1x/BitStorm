using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace WebClient.Views.Components.MultiSelectInput;

public record SelectItem<TItem> (TItem Item, bool Selected);

public partial class MultiSelectInput<TItem>
{
    [Parameter]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public RenderFragment<SelectItem<TItem>> RenderItem { get; set; } =
        context =>
            (RenderTreeBuilder builder) =>
                builder.AddContent(0, context.Item.ToString());

    [Parameter]
    public RenderFragment<SelectItem<TItem>>? RenderMark { get; set; }

    [Parameter]
    public RenderFragment RenderSelect { get; set; }

    [Parameter]
    public IEnumerable<TItem> Values { get; set; }
    [Parameter]
    public EventCallback<IEnumerable<TItem>> ValuesChanged { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    private async Task Clear()
    {
        Values = new List<TItem>();
        await ValuesChanged.InvokeAsync(Values);
    }

    private async Task Add(TItem item)
    {
        Values =
            Values
                .Append(item)
                .ToList();
        await ValuesChanged.InvokeAsync(Values);
    }

    private async Task Remove(TItem item)
    {
        Values =
            Values
                .Where(i => !item.Equals(i))
                .ToList();
        await ValuesChanged.InvokeAsync(Values);
    }

    private Task OnSelect(TItem item)
    {
        if (Values.Contains(item))
        {
            return Remove(item);
        }
        else
        {
            return Add(item);
        }
    }
}
