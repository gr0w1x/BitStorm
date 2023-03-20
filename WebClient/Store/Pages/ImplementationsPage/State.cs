using Fluxor;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.ImplementationsPage;

[FeatureState]
public record ImplementationsPageState: BaseUxServerErrorState<ImplementationsPageState>
{
    public override UxState InitialState => UxState.Idle;
}
