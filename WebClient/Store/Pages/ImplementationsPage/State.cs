using Fluxor;
using Types.Dtos;
using Types.Entities;
using WebClient.Store.Common;
using WebClient.Typing;

namespace WebClient.Store.Pages.ImplementationsPage;

[FeatureState]
public record ImplementationsPageState: BaseUxServerErrorState<ImplementationsPageState>
{
    public override UxState InitialState => UxState.Idle;
    public Task_? Task { get; init; }
    public bool Connected { get; init; }
    public ExecuteCodeResultDto? Output { get; init; }
}
