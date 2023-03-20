using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CommonServer.Asp.HostedServices;

public class RabbitMqProvider
{
    public IConnection? Connection { get; set; }
    public IModel? Model { get; set; }
}

public abstract class RabbitMqService: IHostedService
{
    private readonly IConnectionFactory _factory;
    private readonly ILogger<RabbitMqService> _logger;
    protected readonly RabbitMqProvider Provider;

    protected RabbitMqService(
        IConnectionFactory factory,
        ILogger<RabbitMqService> logger,
        RabbitMqProvider provider
    )
    {
        _factory = factory;
        _logger = logger;
        Provider = provider;
    }

    protected abstract void OnModeling(IModel model);

    private void Stop()
    {
        if (Provider.Model?.IsOpen ?? false)
        {
            Provider.Model.Close();
        }
        if (Provider.Connection?.IsOpen ?? false)
        {
            Provider.Connection.Close();
        }

        Provider.Model = null;
        Provider.Connection = null;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connection = _factory.CreateConnection();
            Provider.Connection = connection;

            var model = connection.CreateModel();
            Provider.Model = model;

            OnModeling(model);

            _logger.LogInformation("Connected to RabbitMQ");
        }
        catch(Exception e)
        {
            _logger.LogError(e.ToString());
            Stop();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();
        return Task.CompletedTask;
    }
}
