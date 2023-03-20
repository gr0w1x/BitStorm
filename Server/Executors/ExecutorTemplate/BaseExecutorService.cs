using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using CommonServer.Data.Messages;
using CommonServer.Utils.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Types.Languages;

namespace ExecutorTemplate;

public abstract class BaseExecutorService<T>: RabbitMqService
{
    private readonly ILogger<BaseExecutorService<T>> _logger;

    protected BaseExecutorService(
        IConnectionFactory factory,
        ILogger<BaseExecutorService<T>> logger,
        RabbitMqProvider provider
    ) : base(factory, logger, provider)
    {
        _logger = logger;
    }

    protected override void OnModeling(IModel model)
    {
        var queue = CodeLanguages.VersionsByType[typeof(T)].Version;

        model.ExecutorQueueDeclare(queue, true);

        var consumer = new AsyncEventingBasicConsumer(model);

        model.BasicConsume(queue, autoAck: true, consumer);

        consumer.Received += OnMessageReceive;
    }

    protected virtual async Task OnMessageReceive(object? _, BasicDeliverEventArgs e)
    {
        var model = Provider.Model!;

        var props = e.BasicProperties;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        ExecuteCodeMessage? execute = JsonSerializer.Deserialize<ExecuteCodeMessage>(message);

        model.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);

        if (execute != null)
        {
            ExecuteCodeResultMessage result = await ExecuteCode(execute);
            var response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result));
            var replyProps = model.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;
            model.BasicPublish(
                exchange: string.Empty,
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: response
            );
        }
    }

    protected abstract Task<ExecuteCodeResultMessage> ExecuteCode(ExecuteCodeMessage code);
}
