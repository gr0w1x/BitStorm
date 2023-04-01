using Types.Dtos;
using Types.Entities;

namespace Types.Hubs;

public interface IExecutionsClient
{
    public Task OnImplementationCodeSaved(ExecuteCodeResultDto code);
    public Task OnTaskSolved(ExecuteCodeResultDto code);
}

public interface IExecutionsServer
{
    public Task SaveImplementationCode(SaveImplementationCodeDto taskImplementation);
    public Task SolveTask(SolveTaskDto solverTask);
}
