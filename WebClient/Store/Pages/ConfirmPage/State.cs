using Fluxor;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.ConfirmPage;

[FeatureState]
public record ConfirmPageState(): BaseUxServerErrorState<ConfirmPageState>
{
    public override UxState InitialState => UxState.Idle;
}
