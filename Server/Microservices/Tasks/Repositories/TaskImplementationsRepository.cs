using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Types.Entities;

namespace Tasks.Repositories;

public interface ITaskImplementationsRepository
{
    Task<TaskImplementation?> Get(Guid taskId, string language, string version);
    Task Delete(Guid taskId, string language, string version);
    Task<IEnumerable<TaskImplementation>> GetByTask(Guid taskId);
}

public class TaskImplementationRepository :
    DbContextRepository<TaskImplementation, TasksContext, Guid>,
    ITaskImplementationsRepository
{
    public TaskImplementationRepository(TasksContext context) : base(context) { }

    public async Task Delete(Guid taskId, string language, string version)
    {
        var item = await Get(taskId, language, version);
        if (item != null)
        {
            Entities.Remove(item);
        }
    }

    public async Task<IEnumerable<TaskImplementation>> GetByTask(Guid taskId)
        => await Entities.Where(impl => impl.TaskId == taskId).ToListAsync();

    public Task<TaskImplementation?> Get(Guid taskId, string language, string version)
        => Entities.FirstOrDefaultAsync(impl =>
            impl.TaskId == taskId &&
            impl.Language == language &&
            impl.Version == version
        );

    protected override DbSet<TaskImplementation> GetEntitiesBy(TasksContext context) => context.Implementations;
}
