using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using CommonServer.Constants;
using CommonServer.Constants.RabbitMqMessageTypes;
using CommonServer.Data.Messages;
using CommonServer.Data.SchedulerJobs;
using CommonServer.Utils.Extensions;
using CommonServer.Utils.RabbitMq;
using Executions.HostedServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using Types.Constants.Errors;
using Types.Dtos;
using Types.Entities;
using Types.Hubs;
using Types.Languages;

namespace Executions.Hubs;

public class RemoveCodeExecutionWrapper: ISchedulerJob
{
    private readonly CodeExecutionResultHandler _handler;
    private readonly ISchedulerJob _job;

    public RemoveCodeExecutionWrapper
    (
        CodeExecutionResultHandler handler,
        ISchedulerJob job
    )
    {
        _handler = handler;
        _job = job;
    }

    public DateTime WhenRun => _job.WhenRun;

    public async Task Run()
    {
        if (
            ExecutionsHub.ConnectionExecution.TryGetValue(_handler.ConnectionId, out CodeExecutionResultHandler? value) &&
                value == _handler
        )
        {
            ExecutionsHub.ConnectionExecution.Remove(_handler.ConnectionId, out _);
        }
        if (!_handler.Executed)
        {
            _handler.Canceled = true;
            await _job.Run();
        }
    }
}

public class CodeExecutionResultHandler: IMessageHandler, ICancelableExecutable
{
    public readonly string ConnectionId;
    protected readonly IHubContext<ExecutionsHub> Context;

    public bool Executed { get; set; }
    public bool Canceled { get; set; }

    public CodeExecutionResultHandler(
        string connectionId,
        IHubContext<ExecutionsHub> context
    )
    {
        ConnectionId = connectionId;
        Context = context;
    }

    public virtual async Task HandleExecution(ExecuteCodeResultDto dto)
    {
        if (!Canceled)
        {
            await
                Context
                    .Clients
                    .Client(ConnectionId)
                    .SendAsync(nameof(IExecutionsHubClient.OnCodeExecuted), dto);
        }
    }

    public async Task Handle(BasicDeliverEventArgs e, string? message)
    {
        if (!Canceled)
        {
            Executed = true;
            ExecutionsHub.ConnectionExecution.Remove(ConnectionId, out _);
            if (message != null)
            {
                var results = JsonSerializer.Deserialize<ExecuteCodeResultDto>(message);
                if (results != null)
                {
                    await HandleExecution(results);
                }
            }
        }
    }
}

public class SaveImplementationAfterCodeExecutionHandler: CodeExecutionResultHandler
{
    private readonly UserClaims _userClaims;
    private readonly SaveImplementationCodeDto _dto;

    private readonly MessageHandlers _handlers;
    private readonly SchedulerController _schedulerController;
    private readonly ExecutionsRabbitMqProvider _provider;

    public SaveImplementationAfterCodeExecutionHandler(
        string connectionId,
        UserClaims userClaims,
        SaveImplementationCodeDto dto,
        IHubContext<ExecutionsHub> context,
        MessageHandlers handlers,
        SchedulerController schedulerController,
        ExecutionsRabbitMqProvider provider
    ): base(connectionId, context)
    {
        _dto = dto;
        _userClaims = userClaims;

        _handlers = handlers;
        _schedulerController = schedulerController;
        _provider = provider;
    }

    public override async Task HandleExecution(ExecuteCodeResultDto dto)
    {
        if (!Canceled)
        {
            await base.HandleExecution(dto);

            if (dto.IsSuccessful())
            {
                var props = _provider.Model!.CreateBasicProperties();
                props.CorrelationId = $"{ConnectionId}.{Guid.NewGuid()}";
                props.ReplyTo = _provider.ExecutorReceiveQueue!;
                props.Type = TasksMessageTypes.SaveTaskImplementation;

                _provider.Model!.BasicPublish(
                    exchange: string.Empty,
                    routingKey: RabbitMqQueries.TasksQuery,
                    mandatory: false,
                    basicProperties: props,
                    body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
                        new SaveImplementationCodeMessage()
                        {
                            Dto = _dto,
                            User = _userClaims
                        }
                    ))
                );

                TaskImplementationSavedHandler handler = new (ConnectionId, Context);
                _handlers.AddHandler(props.CorrelationId, handler);

                _schedulerController.AddJob(
                    new RemoveHandlerJobWrapper(
                        props.CorrelationId,
                        _handlers,
                        new HubTimeoutJob(
                            ConnectionId,
                            Context,
                            handler,
                            TimeSpan.FromSeconds(10),
                            "failed to save task, please try again"
                        )
                    )
                );
            }
            else if (dto.Tests == null)
            {
                await
                    Context
                        .Clients
                        .Client(ConnectionId)
                        .SendAsync(nameof(IExecutionsHubClient.OnError), new ErrorDto("no tests", ExecutionsErrors.NoTests));
            }
        }
    }
}

public class TaskImplementationSavedHandler : IMessageHandler, ICancelableExecutable
{
    public readonly string ConnectionId;
    protected readonly IHubContext<ExecutionsHub> Context;

    public bool Executed { get; set; }
    public bool Canceled { get; set; }

    public TaskImplementationSavedHandler(
        string connectionId,
        IHubContext<ExecutionsHub> context
    )
    {
        ConnectionId = connectionId;
        Context = context;
    }

    public async Task Handle(BasicDeliverEventArgs e, string? message)
    {
        if (!Canceled)
        {
            Executed = true;
            if (message != null)
            {
                var client = Context.Clients.Client(ConnectionId);
                var result = JsonSerializer.Deserialize<ResultMessage>(message);
                if (result != null)
                {
                    if (result.Successful)
                    {
                        var implementation = JsonSerializer.Deserialize<TaskImplementationWithSecretDto>(result.Details);
                        if (implementation != null)
                        {
                            await client.SendAsync(
                                nameof(IExecutionsHubClient.OnImplementationSaved),
                                implementation
                            );
                        }
                    }
                    else
                    {
                        var error = JsonSerializer.Deserialize<ErrorDto>(result.Details);
                        if (error != null)
                        {
                            await client.SendAsync(
                                nameof(IExecutionsHubClient.OnError),
                                error ?? new ErrorDto("internal server error", CommonErrors.InternalServerError)
                            );
                        }
                    }
                }
            }
        }
    }
}

public class ExecutionsHub : Hub, IExecutionsHubServer
{
    public static readonly ConcurrentDictionary<string, CodeExecutionResultHandler> ConnectionExecution = new();

    private readonly MessageHandlers _handlers;
    private readonly ExecutionsRabbitMqProvider _provider;
    private readonly SchedulerController _schedulerController;
    private readonly IHubContext<ExecutionsHub> _context;

    public ExecutionsHub(
        MessageHandlers handlers,
        ExecutionsRabbitMqProvider provider,
        SchedulerController schedulerController,
        IHubContext<ExecutionsHub> context
    )
    {
        _handlers = handlers;
        _provider = provider;
        _schedulerController = schedulerController;
        _context = context;
    }

    [Authorize]
    public async Task SaveImplementationCode(SaveImplementationCodeDto taskImplementation)
    {
        var userClaims = Context.User?.GetUserClaims();
        if (userClaims == null)
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("non authorized", CommonErrors.ForbiddenError)
            );
            return;
        }

        var versionDto = CodeLanguages.GetVersionDto(taskImplementation.Language, taskImplementation.Version);

        if (versionDto == null)
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("no language or version found", CommonErrors.NotFoundError)
            );
            return;
        }

        var correlation = Context.ConnectionId;

        if (ConnectionExecution.ContainsKey(correlation))
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("only one execution per time", ExecutionsErrors.OnlyOneExecution)
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(taskImplementation.Tests))
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("test cases is empty", CommonErrors.ValidationError)
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(taskImplementation.ExampleTests))
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("example test cases is empty", CommonErrors.ValidationError)
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(taskImplementation.InitialSolution))
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("initial solution is empty", CommonErrors.ValidationError)
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(taskImplementation.CompletedSolution))
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("completed solution is empty", CommonErrors.ValidationError)
            );
            return;
        }

        if (taskImplementation.CompletedSolution?.Trim() == taskImplementation.InitialSolution?.Trim())
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("initial and completed solution is equal", CommonErrors.ValidationError)
            );
            return;
        }

        if (taskImplementation.Tests?.Trim() == taskImplementation.ExampleTests?.Trim())
        {
            await Clients.Caller.SendAsync(
                nameof(ICommonHubClient.OnError),
                new ErrorDto("example test cases and really used tests are equal", CommonErrors.ValidationError)
            );
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
                    taskImplementation.PreloadedCode ?? "",
                    taskImplementation.CompletedSolution,
                    taskImplementation.Tests
                )
            ))
        );

        SaveImplementationAfterCodeExecutionHandler handler = new (
            correlation,
            userClaims,
            taskImplementation,
            _context,
            _handlers,
            _schedulerController,
            _provider
        );
        _handlers.AddHandler(correlation, handler);

        ConnectionExecution[correlation] = handler;

        _schedulerController.AddJob(
            new RemoveHandlerJobWrapper(
                correlation,
                _handlers,
                new RemoveCodeExecutionWrapper(
                    handler,
                    new HubTimeoutJob(
                        correlation,
                        _context,
                        handler,
                        TimeSpan.FromSeconds(10) + versionDto.ExecutionTimeout,
                        $"executor for language {taskImplementation.Language} and version {taskImplementation.Version} not responded"
                    )
                )
            )
        );
    }

    public Task SolveTask(SolveTaskDto solverTask)
    {
        throw new NotImplementedException();
    }
}
