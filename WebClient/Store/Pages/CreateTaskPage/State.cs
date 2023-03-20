using Fluxor;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.CreateTaskPage;

[FeatureState]
public record CreateTaskPageState(): BaseUxServerErrorState<CreateTaskPageState>
{
    public override UxState InitialState => UxState.Editable;
}
