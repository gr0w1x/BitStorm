using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Types.Dtos;
using Types.Entities;

namespace Tasks.Repositories;

public interface ITasksRepository: IRepository<Task_, Guid>
{
    Task<Task_?> GetByTitle(string title);
    Task<TasksInfoDto> GetTasksInfo(GetTasksInfoDto dto);
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

    public async Task<TasksInfoDto> GetTasksInfo(GetTasksInfoDto dto)
    {
        var query = Entities.Include(task => task.Tags).AsQueryable();

        if (dto.Levels != null)
        {
            query = query
                .Where(task => dto.Levels.Contains(task.Level));
        }
        if (dto.StatusOptions != null)
        {
            switch (dto.StatusOptions)
            {
                case GetTasksInfoDto.StatusOptions.OnlyBeta:
                {
                    query = query.Where(task => task.Beta);
                    break;
                }
                case GetTasksInfoDto.StatusOptions.OnlyApproved:
                {
                    query = query.Where(task => !task.Beta);
                    break;
                }
            }
        }
        if (dto.Query != null)
        {
            query.Where(task => task.Title.Contains(dto.Query));
        }
        if (dto.Languages != null)
        {
            query = query
                .Include(task => task.Implementations)
                .Where(
                    task => task.Implementations.Any(implementation => dto.Languages.Contains(implementation.Language))
                );
        }
        if (dto.Tags != null)
        {
            query = query
                .Where(task => dto.Tags.All(tag => task.Tags.Any(t => t.Id == tag)));
        }

        var total = query.Count();
        var tags = query
            .SelectMany(task => task.Tags)
            .GroupBy(tag => tag.Id)
            .ToDictionary(group => group.Key, group => group.Count());

        return new TasksInfoDto()
        {
            Total = total,
            Tags = tags
        };
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
