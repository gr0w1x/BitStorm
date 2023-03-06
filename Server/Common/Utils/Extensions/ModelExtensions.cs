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
}
