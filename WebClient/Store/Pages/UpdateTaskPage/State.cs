using Fluxor;
using Types.Entities;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.UpdateTaskPage;

[FeatureState]
public record UpdateTaskPageState(): BaseUxServerErrorState<UpdateTaskPageState>
{
    public override UxState InitialState => UxState.Editable;

    public Task_? Task { get; init; }
}
