using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Store.Pages.SignPageBase;

public record SignPageBaseState<T>: IHasUxState
{
    public UxState UxState { get; init; }
    public string? ServerError { get; init; }
}
