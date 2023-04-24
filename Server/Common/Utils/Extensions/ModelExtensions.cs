using RabbitMQ.Client;

namespace CommonServer.Utils.Extensions;

public static class ModelExtensions
{
    public static QueueDeclareOk MailerRequestQueueDeclare(this IModel model, string name)
    {
        return model.QueueDeclare(
            queue: name,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
    }

    public static QueueDeclareOk TasksQueueDeclare(this IModel model, string name)
    {
        return model.QueueDeclare(
            queue: name,
            durable: false,
            exclusive: false,
            autoDelete: false
        );
    }

    public static QueueDeclareOk ExecutorQueueDeclare(this IModel model, string name, bool forExecutor = false)
    {
        var declare = model.QueueDeclare(
            queue: name,
            durable: false,
            exclusive: false,
            autoDelete: false
        );
        if (forExecutor)
        {
            model.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false
            );
        }
        return declare;
    }
}
