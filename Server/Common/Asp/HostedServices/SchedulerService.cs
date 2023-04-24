using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommonServer.Asp.HostedServices;

public interface ISchedulerJob
{
    DateTime WhenRun { get; }
    Task Run ();
}

public class CallbackJob: ISchedulerJob
{
    private readonly Func<Task> _action;

    public CallbackJob(DateTime when, Func<Task> action)
    {
        WhenRun = when;
        _action = action;
    }

    public CallbackJob(TimeSpan timeout, Func<Task> action): this(DateTime.Now + timeout, action) { }

    public DateTime WhenRun { get; }
    public Task Run() => _action();
}

public class SchedulerController
{
    private SchedulerService? _scheduler;

    internal void Bind(SchedulerService? scheduler)
    {
        _scheduler = scheduler;
    }

    public void AddJob(ISchedulerJob job)
    {
        if (_scheduler == null)
        {
            throw new NullReferenceException();
        }
        _scheduler.AddJob(job);
    }
}

public class SchedulerService : BackgroundService
{
    private readonly PriorityQueue<ISchedulerJob, DateTime> _queue = new();
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly object _lock = new();
    private readonly ILogger<SchedulerController> _logger;
    private readonly SchedulerController _controller;

    public SchedulerService(SchedulerController controller, ILogger<SchedulerController> logger)
    {
        _logger = logger;
        _controller = controller;
        _controller.Bind(this);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            do
            {
                if (_queue.Count != 0)
                {
                    var now = DateTime.Now;
                    var job = _queue.Peek();
                    if (job.WhenRun > now)
                    {
                        try
                        {
                            await Task.Delay(job.WhenRun - now, _cancellationTokenSource.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            continue;
                        }
                    }
                    lock (_lock)
                    {
                        if (_queue.Peek() != job)
                        {
                            continue;
                        }
                        if (!stoppingToken.IsCancellationRequested)
                        {
                            _queue.Dequeue();
                        }
                    }
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            await job.Run();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await Task.Delay(-1, _cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException) { }
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }
        finally
        {
            _controller.Bind(null);
        }
    }

    public void AddJob(ISchedulerJob job)
    {
        lock (_lock)
        {
            _queue.Enqueue(job, job.WhenRun);
            if (_queue.Count == 1 || job.WhenRun < _queue.Peek().WhenRun)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
    }
}
