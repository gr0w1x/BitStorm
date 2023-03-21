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

    private readonly ITagsRepository _tagsRepository;

    public TasksRepository(TasksContext context, ITagsRepository tagsRepository): base(context)
    {
        _tagsRepository = tagsRepository;
    }

    public override Task<Task_?> GetById(Guid id) =>
        Entities
            .Include(task => task.Likes)
            .Include(task => task.Tags)
            .FirstOrDefaultAsync(task => task.Id == id);

    public Task<Task_?> GetByTitle(string title) =>
        Entities.FirstOrDefaultAsync(
            task => task.Title == title
        );

    public override async Task Create(params Task_[] entities)
    {
        var tags = entities.SelectMany(task => task.Tags).Select(tag => tag.Id).ToHashSet();
        var existed = (await _tagsRepository.GetByIds(tags.ToArray())).ToDictionary(tag => tag.Id);
        foreach(var task in entities)
        {
            task.Tags =
                task.Tags
                    .ConvertAll(tag =>
                    {
                        existed.TryGetValue(tag.Id, out TaskTag? t);
                        return t ?? tag;
                    });
        }
        await base.Create(entities);
    }
}
