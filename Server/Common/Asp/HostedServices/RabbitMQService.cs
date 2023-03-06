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
    private readonly RabbitMqProvider _provider;

    protected RabbitMqService(
        IConnectionFactory factory,
        ILogger<RabbitMqService> logger,
        RabbitMqProvider provider
    )
    {
        _factory = factory;
        _logger = logger;
        _provider = provider;
    }

    protected abstract void OnModeling(IModel model);

    private void Stop()
    {
        if (_provider.Model?.IsOpen ?? false)
        {
            _provider.Model.Close();
        }
        if (_provider.Connection?.IsOpen ?? false)
        {
            _provider.Connection.Close();
        }

        _provider.Model = null;
        _provider.Connection = null;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connection = _factory.CreateConnection();
            _provider.Connection = connection;

            var model = connection.CreateModel();
            _provider.Model = model;

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
