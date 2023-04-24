using CommonServer.Asp.HostedServices;
using CommonServer.Utils.RabbitMq;

namespace CommonServer.Data.SchedulerJobs;

public class RemoveHandlerJobWrapper: ISchedulerJob
{
    private readonly string _correlationId;
    private readonly MessageHandlers _handlers;
    private readonly ISchedulerJob _job;

    public RemoveHandlerJobWrapper
    (
        string correlationId,
        MessageHandlers handlers,
        ISchedulerJob job
    )
    {
        _correlationId = correlationId;
        _handlers = handlers;
        _job = job;
    }

    public DateTime WhenRun => _job.WhenRun;

    public async Task Run()
    {
        _handlers.RemoveHandler(_correlationId);
        await _job.Run();
    }
}
