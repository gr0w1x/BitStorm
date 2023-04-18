using CommonServer.Asp.HostedServices;
using Microsoft.AspNetCore.SignalR;
using Types.Constants.Errors;
using Types.Dtos;
using Types.Hubs;

namespace CommonServer.Data.SchedulerJobs;

public interface ICancelableExecutable
{
    bool Executed { get; set; }
    bool Canceled { get; set; }
}

public class HubTimeoutJob: ISchedulerJob
{
    protected readonly string _connectionId;
    protected readonly IHubContext<Hub> _context;
    protected readonly ICancelableExecutable _executable;
    protected readonly string? _error;

    public HubTimeoutJob(
        string connectionId,
        IHubContext<Hub> context,
        ICancelableExecutable executable,
        TimeSpan timeout,
        string? error = null
    )
    {
        _connectionId = connectionId;
        _context = context;
        _executable = executable;
        _error = error;
        WhenRun = DateTime.Now + timeout;
    }

    public DateTime WhenRun { get; }

    public virtual async Task Run()
    {
        if (!_executable.Executed)
        {
            _executable.Canceled = true;
            await _context
                .Clients
                .Client(_connectionId)
                .SendAsync(
                    nameof(ICommonHubClient.OnError),
                    new ErrorDto(_error ?? "timeout", CommonErrors.TimeoutError)
                );
        }
    }
}
