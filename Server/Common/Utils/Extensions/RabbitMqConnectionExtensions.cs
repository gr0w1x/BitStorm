using RabbitMQ.Client;

namespace CommonServer.Utils.Extensions;

public static class RabbitMqConnectionExtensions
{
    public static ConnectionFactory DefaultRabbitMqConnection(this ConnectionFactory factory)
    {
        factory.HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST")!;
        factory.Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"));
        factory.UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER")!;
        factory.Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!;
        factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);
        factory.RequestedConnectionTimeout = TimeSpan.FromSeconds(60);
        factory.VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST")!;
        factory.DispatchConsumersAsync = true;
        return factory;
    }
}
