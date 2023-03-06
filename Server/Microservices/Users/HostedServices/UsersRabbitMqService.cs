using CommonServer.Asp.HostedServices;
using CommonServer.Utils.Extensions;
using RabbitMQ.Client;

namespace Users.HostedServices;

public class UsersRabbitMqService: RabbitMqService
{
    public readonly string MailRequestQueue;

    public UsersRabbitMqService(
        IConfiguration configuration,
        IConnectionFactory factory,
        RabbitMqProvider provider,
        ILogger<UsersRabbitMqService> logger
    ): base(factory, logger, provider)
    {
        MailRequestQueue = configuration["MAILER_REQUEST_QUEUE"]!;
    }

    protected override void OnModeling(IModel model)
    {
        model.MailerRequestQueueDeclare(MailRequestQueue);
    }
}
