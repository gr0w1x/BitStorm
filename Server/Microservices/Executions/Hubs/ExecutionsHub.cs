using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using CommonServer.Constants;
using CommonServer.Data.SchedulerJobs;
using Executions.HostedServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Types.Constants.Errors;
using Types.Dtos;
using Types.Entities;
using Types.Hubs;
using Types.Languages;

namespace Executions.Hubs;

public class RemoveCodeExecutionTimeoutJob: HubTimeoutJob
{
    public RemoveCodeExecutionTimeoutJob(
        string connectionId,
        IHubContext<Hub> context,
        ICancelableExecutable executable,
        TimeSpan timeout,
        string? error = null
    ) : base(connectionId, context, executable, timeout, error) { }

    public override async Task Run()
    {
        if (!_executable.Executed)
        {
            await base.Run();
            ExecutionsHub.ConnectionExecution.Remove(_connectionId, out _);
        }
    }
}

public class CodeExecutionResultHandler: ICancelableExecutable
{
    private readonly string _connectionId;
    private readonly IHubContext<ExecutionsHub> _context;

    public bool Executed { get; set; }
    public bool Canceled { get; set; }

    public CodeExecutionResultHandler(
        string connectionId,
        IHubContext<ExecutionsHub> context
    )
    {
        _connectionId = connectionId;
        _context = context;
    }

    public virtual async Task Handle(ExecuteCodeResultDto dto)
    {
        if (!Canceled)
        {
            Executed = true;
            await _context
                .Clients
                .Client(_connectionId)
                .SendAsync(nameof(IExecutionsHubClient.OnCodeExecuted), dto);
        }
    }
}

public class SaveImplementationHandler: CodeExecutionResultHandler
{
    private readonly SaveImplementationCodeDto _dto;

    public SaveImplementationHandler(
        string connectionId,
        IHubContext<ExecutionsHub> context,
        SaveImplementationCodeDto dto
    ): base(connectionId, context)
    {
        _dto = dto;
    }

    public override async Task Handle(ExecuteCodeResultDto dto)
    {
        if (!Canceled)
        {
            await base.Handle(dto);
            // TODO: add sending a message to rabbitmq to save a implementation and handle it
        }
    }
}

[Authorize]
public class ExecutionsHub : Hub, IExecutionsHubServer
{
    public static readonly ConcurrentDictionary<string, CodeExecutionResultHandler> ConnectionExecution = new();

    private readonly ExecutionsRabbitMqProvider _provider;
    private readonly SchedulerService.SchedulerController _schedulerController;
    private readonly IHubContext<ExecutionsHub> _context;

    public ExecutionsHub(ExecutionsRabbitMqProvider provider, SchedulerService.SchedulerController schedulerController, IHubContext<ExecutionsHub> context)
    {
        _provider = provider;
        _schedulerController = schedulerController;
        _context = context;
    }

    public async Task SaveImplementationCode(SaveImplementationCodeDto taskImplementation)
    {
        var versionDto = CodeLanguages.GetVersionDto(taskImplementation.Language, taskImplementation.Version);

        if (versionDto == null)
        {
            await Clients.Caller.SendAsync(nameof(ICommonHubClient.OnError), new ErrorDto("no language or version found", CommonErrors.NotFoundError));
            return;
        }

        var correlation = Context.ConnectionId;

        if (ConnectionExecution.ContainsKey(correlation))
        {
            await Clients.Caller.SendAsync(nameof(ICommonHubClient.OnError), new ErrorDto("only one execution per time", ExecutionsErrors.OnlyOneExecution));
            return;
        }

        var props = _provider.Model!.CreateBasicProperties();
        props.CorrelationId = correlation;
        props.ReplyTo = _provider.ExecutorReceiveQueue!;

        _provider.Model!.BasicPublish(
            exchange: string.Empty,
            routingKey: RabbitMqQueries.ExecutorQuery(taskImplementation.Language, taskImplementation.Version),
            mandatory: false,
            basicProperties: props,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
                new ExecuteCodeDto(
                    taskImplementation.Preloaded ?? "",
                    taskImplementation.CompletedSolution,
                    taskImplementation.TestCases
                )
            ))
        );

        SaveImplementationHandler handler = new (correlation, _context, taskImplementation);

        _schedulerController.AddJob(new RemoveCodeExecutionTimeoutJob(
            correlation,
            _context,
            handler,
            TimeSpan.FromSeconds(10) + versionDto.ExecutionTimeout,
            $"executor for language {taskImplementation.Language} and version {taskImplementation.Version} not responded"
        ));
    }

    public Task SolveTask(SolveTaskDto solverTask)
    {
        throw new NotImplementedException();
    }
}
