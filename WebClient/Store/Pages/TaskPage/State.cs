using Fluxor;
using Types.Dtos;
using WebClient.Models;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.TaskPage;

[FeatureState]
public record TaskPageState: BaseUxServerErrorState<TaskPageState>
{
    public override UxState InitialState => UxState.Idle;

    public TaskCardModel? Task { get; set; }
}
