using Fluxor;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.UserMenu;

[FeatureState]
public record UserMenuState: IHasUxState
{
    public UxState UxState { get; init; }

    public UxState InitialState => UxState;

    public UserMenuState()
    {
        UxState = InitialState;
    }
}
