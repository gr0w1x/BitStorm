using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebClient.Views.Components.SelectInput;

public partial class SelectInput<TItem>: InputSelect<TItem>
    where TItem: IComparable
{
    [Parameter]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public Func<TItem, bool, RenderFragment> RenderItem { get; set; } =
        (item, _) => (
            builder => builder.AddContent(0, item.ToString())
        );

    private Func<TItem?, RenderFragment>? _renderSelected;
    [Parameter]
    public Func<TItem?, RenderFragment> RenderSelected
    {
        get
        {
            _renderSelected ??= (item) =>
                {
                    if (item != null)
                    {
                        return RenderItem(item, true);
                    }
                    else
                    {
                        return _ => { };
                    }
                };
            return _renderSelected;
        }
        set => _renderSelected = value;
    }

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
