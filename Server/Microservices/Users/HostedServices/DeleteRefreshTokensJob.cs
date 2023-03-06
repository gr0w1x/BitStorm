using CommonServer.Asp.HostedServices;
using Users.Services;

namespace Users.HostedServices;

public class DeleteRefreshTokensJob: CronJobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeleteRefreshTokensJob> _logger;

    public DeleteRefreshTokensJob(
        ICronOptions<DeleteRefreshTokensJob> options,
        IServiceProvider serviceProvider,
        ILogger<DeleteRefreshTokensJob> logger
    ) : base(options.Cron)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task Process()
    {
        using var scope =
            _serviceProvider
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

        _logger.LogInformation("Deleting refreshing tokens...");

        await scope
            .ServiceProvider
            .GetRequiredService<AuthService>()
            .DeleteExpired();
    }
}
