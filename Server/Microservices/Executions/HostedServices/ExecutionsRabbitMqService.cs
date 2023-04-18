using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using CommonServer.Utils.Extensions;
using Executions.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Types.Dtos;

namespace Executions.HostedServices;

public class ExecutionsRabbitMqProvider: RabbitMqProvider
{
    public string? ExecutorReceiveQueue { get; set; }
}

public class ExecutionsRabbitMqService : RabbitMqService<ExecutionsRabbitMqProvider>
{
    public ExecutionsRabbitMqService(
        IConnectionFactory factory,
        ILogger<RabbitMqService<ExecutionsRabbitMqProvider>> logger,
        ExecutionsRabbitMqProvider provider
    ) : base(factory, logger, provider) { }

    protected override void OnModeling(IModel model)
    {
        Provider.ExecutorReceiveQueue = $"executions-{new Guid()}";
        model.ExecutorQueueDeclare(Provider.ExecutorReceiveQueue);

        var consumer = new AsyncEventingBasicConsumer(model);

        model.BasicConsume(Provider.ExecutorReceiveQueue, autoAck: true, consumer);

        consumer.Received += OnExecutorMessageReceive;
    }

    private async Task OnExecutorMessageReceive(object? _, BasicDeliverEventArgs e)
    {
        var props = e.BasicProperties;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        ExecuteCodeResultDto? executeResult = JsonSerializer.Deserialize<ExecuteCodeResultDto>(message);

        if (executeResult != null && ExecutionsHub.ConnectionExecution.Remove(props.CorrelationId, out CodeExecutionResultHandler? handler))
        {
            await handler.Handle(executeResult);
        }
    }
}
