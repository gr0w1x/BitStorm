using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using CommonServer.Utils.Extensions;
using Executions.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Types.Dtos;
using Types.Hubs;

namespace Executions.HostedServices;

public class ExecutionsRabbitMqProvider: RabbitMqProvider
{
    public string? ExecutorReceiveQueue { get; set; }
}

public class ExecutionsRabbitMqService : RabbitMqService<ExecutionsRabbitMqProvider>
{
    private readonly IHubContext<ExecutionsHub> ExecutionsContext;

    public ExecutionsRabbitMqService(
        IConnectionFactory factory,
        ILogger<RabbitMqService<ExecutionsRabbitMqProvider>> logger,
        ExecutionsRabbitMqProvider provider,
        IHubContext<ExecutionsHub> context
    ) : base(factory, logger, provider)
    {
        ExecutionsContext = context;
    }

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
        var model = Provider.Model!;

        var props = e.BasicProperties;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        ExecuteCodeResultDto? executeResult = JsonSerializer.Deserialize<ExecuteCodeResultDto>(message);

        if (executeResult != null)
        {
            await ExecutionsContext
                .Clients
                .Client(props.CorrelationId)
                .SendAsync(nameof(IExecutionsClient.OnImplementationCodeSaved), executeResult);
        }
    }
}
