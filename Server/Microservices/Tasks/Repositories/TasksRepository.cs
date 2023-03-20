using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Types.Entities;

namespace Tasks.Repositories;

public interface ITasksRepository: IRepository<Task_, Guid>
{
    Task<Task_?> GetByTitle(string title);
}

public class TasksRepository:
    DbContextRepository<Task_, TasksContext, Guid>,
    ITasksRepository
{
    protected override DbSet<Task_> GetEntitiesBy(TasksContext context) => context.Tasks;

    public override Task<Task_?> GetById(Guid id) =>
        Entities
            .Include(task => task.Likes)
            .FirstOrDefaultAsync(task => task.Id == id);

    public Task<Task_?> GetByTitle(string title) =>
        Entities.FirstOrDefaultAsync(
            task => task.Title == title
        );

    public TasksRepository(TasksContext context): base(context) { }
}
