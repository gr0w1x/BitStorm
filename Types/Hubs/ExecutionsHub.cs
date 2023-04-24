using Types.Dtos;
using Types.Entities;

namespace Types.Hubs;

public interface IExecutionsHubClient: ICommonHubClient
{
    public Task OnCodeExecuted(ExecuteCodeResultDto code);
    public Task OnImplementationSaved(TaskImplementationWithSecretDto implementation);
    public Task OnTaskSolved(ExecuteCodeResultDto code);
}

public interface IExecutionsHubServer
{
    public Task SaveImplementationCode(SaveImplementationCodeDto taskImplementation);
    public Task SolveTask(SolveTaskDto solverTask);
}
