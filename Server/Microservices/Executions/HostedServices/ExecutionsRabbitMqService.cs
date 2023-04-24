using CommonServer.Asp.HostedServices;
using CommonServer.Utils.Extensions;
using CommonServer.Utils.RabbitMq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Executions.HostedServices;

public class ExecutionsRabbitMqProvider: RabbitMqProvider
{
    public string? ExecutorReceiveQueue { get; set; }
}

public class ExecutionsRabbitMqService : RabbitMqService<ExecutionsRabbitMqProvider>
{
    private readonly MessageHandlers _handlers;

    public ExecutionsRabbitMqService(
        MessageHandlers handlers,
        IConnectionFactory factory,
        ILogger<RabbitMqService<ExecutionsRabbitMqProvider>> logger,
        ExecutionsRabbitMqProvider provider
    ) : base(factory, logger, provider)
    {
        _handlers = handlers;
    }

    protected override void OnModeling(IModel model)
    {
        Provider.ExecutorReceiveQueue = $"executions-{Guid.NewGuid()}";
        model.ExecutorQueueDeclare(Provider.ExecutorReceiveQueue);

        var consumer = new AsyncEventingBasicConsumer(model);

        model.BasicConsume(Provider.ExecutorReceiveQueue, autoAck: true, consumer);

        consumer.Received += OnExecutorMessageReceive;
    }

    private async Task OnExecutorMessageReceive(object? _, BasicDeliverEventArgs e)
    {
        await _handlers.HandleMessage(e);
    }
}
