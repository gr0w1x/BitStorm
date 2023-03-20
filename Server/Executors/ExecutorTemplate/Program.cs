using CommonServer.Asp.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using CommonServer.Utils.Extensions;
using Microsoft.Extensions.Hosting;

namespace ExecutorTemplate;

public static class ExecutorTemplate
{
    public static void Main<TExecutor, TType>(string[] args)
        where TExecutor: BaseExecutorService<TType>
    {
        IHost host
            = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConnectionFactory>(new ConnectionFactory().DefaultRabbitMqConnection());
                    services.AddSingleton<RabbitMqProvider>();
                    services.AddSingleton<IConfiguration>(new ConfigurationManager().AddEnvironmentVariables().Build());
                    services.AddSingleton<ILoggerFactory, LoggerFactory>();
                    services.AddHostedService<TExecutor>();
                })
                .Build();

        host.Run();
    }
}
