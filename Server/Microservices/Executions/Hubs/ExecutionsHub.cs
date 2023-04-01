using System.Text;
using System.Text.Json;
using CommonServer.Constants;
using Executions.HostedServices;
using Microsoft.AspNetCore.SignalR;
using Types.Dtos;
using Types.Entities;
using Types.Hubs;

namespace Executions.Hubs;

public class ExecutionsHub : Hub, IExecutionsServer
{
    private readonly ExecutionsRabbitMqProvider _provider;

    public ExecutionsHub(ExecutionsRabbitMqProvider provider)
    {
        _provider = provider;
    }

    public Task SaveImplementationCode(SaveImplementationCodeDto taskImplementation)
    {
        var props = _provider.Model!.CreateBasicProperties();
        props.CorrelationId = Context.ConnectionId;
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

        return Task.CompletedTask;
    }

    public Task SolveTask(SolveTaskDto solverTask)
    {
        throw new NotImplementedException();
    }
}
