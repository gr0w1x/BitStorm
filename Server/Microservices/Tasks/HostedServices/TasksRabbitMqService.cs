using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using CommonServer.Constants;
using CommonServer.Constants.RabbitMqMessageTypes;
using CommonServer.Data.Messages;
using CommonServer.Utils.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tasks.Services;

namespace Tasks.HostedServices;

public class TasksRabbitMqService: RabbitMqService<RabbitMqProvider>
{
    private readonly IServiceScopeFactory _scopedFactory;

    public TasksRabbitMqService(
        IConnectionFactory factory,
        ILogger<RabbitMqService<RabbitMqProvider>> logger,
        RabbitMqProvider provider,
        IServiceScopeFactory scopedFactory
    ) : base(factory, logger, provider)
    {
        _scopedFactory = scopedFactory;
    }

    protected override void OnModeling(IModel model)
    {
        model.ExecutorQueueDeclare(RabbitMqQueries.TasksQuery);

        var consumer = new AsyncEventingBasicConsumer(model);

        model.BasicConsume(RabbitMqQueries.TasksQuery, autoAck: true, consumer);

        consumer.Received += OnExecutorMessageReceive;
    }

    private async Task OnExecutorMessageReceive(object? _, BasicDeliverEventArgs e)
    {
        var props = e.BasicProperties;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        if (message != null)
        {
            switch (props.Type)
            {
                case TasksMessageTypes.SaveTaskImplementation:
                {
                    var saveImplementation = JsonSerializer.Deserialize<SaveImplementationCodeMessage>(message);
                    if (saveImplementation != null)
                    {
                        using var scope = _scopedFactory.CreateScope();
                        var tasksService = scope.ServiceProvider.GetRequiredService<TasksService>();
                        var (Implementation, Error) = await tasksService.SaveTaskImplementation(saveImplementation.Dto, saveImplementation.User);

                        var replyProps = Provider.Model!.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        replyProps.Type = props.Type;

                        Provider.Model!.BasicPublish(
                            exchange: string.Empty,
                            routingKey: props.ReplyTo,
                            mandatory: false,
                            basicProperties: replyProps,
                            body: Encoding.UTF8.GetBytes(
                                JsonSerializer.Serialize(
                                    new ResultMessage(Error == null,
                                        Error != null
                                            ? JsonSerializer.Serialize(Error)
                                            : JsonSerializer.Serialize(Implementation)
                                    )
                                )
                            )
                        );
                    }
                    break;
                }
            }
        }
    }
}
