using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Types.Entities;

namespace Tasks.Repositories;

public interface ITagsRepository: IRepository<TaskTag, string>
{
    Task<List<TaskTag>> GetByIds(string[] ids);
}

public class TagsRepository:
    DbContextRepository<TaskTag, TasksContext, string>,
    ITagsRepository
{
    public TagsRepository(TasksContext context) : base(context) { }

    protected override DbSet<TaskTag> GetEntitiesBy(TasksContext context) => context.Tags;

    public Task<List<TaskTag>> GetByIds(string[] ids) => Entities.Where(tag => ids.Contains(tag.Id)).ToListAsync();
}
