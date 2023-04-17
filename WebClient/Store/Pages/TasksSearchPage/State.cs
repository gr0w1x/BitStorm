using Fluxor;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.TasksSearchPage;

[FeatureState]
public record TasksSearchPageState: BaseUxServerErrorState<TasksSearchPageState>
{
    public override UxState InitialState => UxState.Idle;
}
