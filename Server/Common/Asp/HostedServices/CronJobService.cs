using Microsoft.Extensions.Hosting;
using NCrontab;

namespace CommonServer.Asp.HostedServices;

public abstract class CronJobService : BackgroundService
{
    private readonly CrontabSchedule _schedule;
    private DateTime _nextRun;

    protected CronJobService(string cron)
    {
        _schedule = CrontabSchedule.Parse(cron, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            var now = DateTime.Now;
            _nextRun = _schedule.GetNextOccurrence(now);
            await Task.Delay(_nextRun - now, stoppingToken);
            if (!stoppingToken.IsCancellationRequested)
            {
                await Process();
            }
        }
        while (!stoppingToken.IsCancellationRequested);
    }

    protected abstract Task Process();
}

public interface ICronOptions<T>
{
    string Cron { get; }
}

public class CronOptions<T> : ICronOptions<T>
{
    public string Cron { get; set; }
}
