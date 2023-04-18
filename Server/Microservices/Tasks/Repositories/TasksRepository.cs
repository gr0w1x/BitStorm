using System.Linq.Expressions;
using CommonServer.Data.Repositories;
using CommonServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Types.Dtos;
using Types.Entities;

namespace Tasks.Repositories;

public interface ITasksRepository: IRepository<Task_, Guid>
{
    Task<Task_?> GetByTitle(string title);
    Task<TasksInfoDto> GetTasksInfo(GetTasksInfoDto dto, bool viewAll, Guid? viewer);
    Task<IEnumerable<Task_>> GetTasks(GetTasksDto dto, bool viewAll, Guid? viewer);
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

    private IQueryable<Task_> TasksSearchQuery(GetTasksInfoDto dto, bool viewAll, Guid? viewer)
    {
        var query = Entities.Include(task => task.Tags).AsQueryable();

        if (dto.Query != null)
        {
            query = query.Where(task => EF.Functions.Like(task.Title + task.Description, $"%{dto.Query}%"));
        }
        if (!viewAll)
        {
            if (viewer != null)
            {
                var v = viewer.Value;
                query = query.Where(task => task.AuthorId == v);
            }
            else
            {
                query = query.Where(task => task.Visibility == TaskVisibility.Public);
            }
        }
        if (dto.Levels != null)
        {
            query = query
                .Where(task => dto.Levels.Contains(task.Level));
        }
        switch (dto.Status)
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
            foreach (var _tag in dto.Tags)
            {
                query = query
                    .Where(
                        task => task.Tags.Any(t => t.Id == _tag)
                    );
            }
        }

        return query;
    }

    public async Task<TasksInfoDto> GetTasksInfo(GetTasksInfoDto dto, bool viewAll, Guid? viewer)
    {
        var query = TasksSearchQuery(dto, viewAll, viewer);

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

    public async Task<IEnumerable<Task_>> GetTasks(GetTasksDto dto, bool viewAll, Guid? viewer)
    {
        var query = TasksSearchQuery(dto, viewAll, viewer);

        Func
        <
            IQueryable<Task_>,
            Expression<Func<Task_, object>>,
            IOrderedQueryable<Task_>
        > sortStrategy = (
            (dto.Sort == GetTasksDto.SortStrategy.Name && dto.Sort == GetTasksDto.SortStrategy.Level)
                ? ((IQueryable<Task_> query, Expression<Func<Task_, object>> key) => (dto.Inversed ?? false) ? query.OrderByDescending(key) : query.OrderBy(key))
                : ((IQueryable<Task_> query, Expression<Func<Task_, object>> key) => (dto.Inversed ?? false) ? query.OrderBy(key) : query.OrderByDescending(key))
        );

        Expression<Func<Task_, object>> mapper =
            dto.Sort == GetTasksDto.SortStrategy.LastUpdated
                ? task => task.UpdatedAt ?? task.CreatedAt
                : dto.Sort == GetTasksDto.SortStrategy.Level
                ? task => task.Level
                : dto.Sort == GetTasksDto.SortStrategy.Likes
                ? task => task.Likes
                : task => task.Title;

        query = sortStrategy(query, mapper);

        return query
            .SkipAndTake(dto.Skip, dto.Take)
            .ToList();
    }

    public override Task<Task_?> GetById(Guid id) =>
        Entities
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

    public override async Task Update(params Task_[] entities)
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
        await base.Update(entities);
    }
}
