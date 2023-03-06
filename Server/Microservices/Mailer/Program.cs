using CommonServer.Asp.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using CommonServer.Utils.Extensions;
using Microsoft.Extensions.Hosting;

namespace Mailer;

public static class Program
{
    public static void Main(string[] args)
    {
        IHost host
            = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConnectionFactory>(new ConnectionFactory().DefaultRabbitMqConnection());
                    services.AddSingleton<RabbitMqProvider>();
                    services.AddSingleton<IConfiguration>(new ConfigurationManager().AddEnvironmentVariables().Build());
                    services.AddSingleton<ILoggerFactory, LoggerFactory>();
                    services.AddHostedService<MailerRabbitMqService>();
                })
                .Build();

        host.Run();
    }
}
