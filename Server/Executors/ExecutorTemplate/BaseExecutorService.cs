using System.Text;
using System.Text.Json;
using CommonServer.Constants;
using CommonServer.Asp.HostedServices;
using CommonServer.Utils.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Types.Dtos;
using Types.Languages;

namespace ExecutorTemplate;

public abstract class BaseExecutorService<T>: RabbitMqService<RabbitMqProvider>
{
    protected readonly ILogger<BaseExecutorService<T>> ExecutorLogger;

    protected BaseExecutorService(
        IConnectionFactory factory,
        ILogger<BaseExecutorService<T>> logger,
        RabbitMqProvider provider
    ) : base(factory, logger, provider)
    {
        ExecutorLogger = logger;
    }

    protected override void OnModeling(IModel model)
    {
        var version = CodeLanguages.VersionsByType[typeof(T)];

        var queue = RabbitMqQueries.ExecutorQuery(version.Language, version.Version);

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
        ExecuteCodeDto? execute = JsonSerializer.Deserialize<ExecuteCodeDto>(message);

        if (execute != null)
        {
            ExecuteCodeResultDto result = await ExecuteCode(execute);
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

    protected abstract Task<ExecuteCodeResultDto> ExecuteCode(ExecuteCodeDto code);
}
