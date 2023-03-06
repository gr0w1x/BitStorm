using CommonServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CommonServer.Data.Repositories;

public abstract class DbContextRepository<TEntity, TContext, TKey>: IRepository<TEntity, TKey>
    where TEntity: class
    where TContext: DbContext
{
    private readonly TContext _context;

    protected DbContextRepository(TContext context)
    {
        _context = context;
    }

    protected abstract DbSet<TEntity> GetEntitiesBy(TContext context);

    protected DbSet<TEntity> Entities => GetEntitiesBy(_context);

    public async Task<TEntity?> GetById(TKey id) =>
        await Entities.FindAsync(id);

    public async Task<IEnumerable<TEntity>> Select(int? skip, int? take) =>
        await Entities.SkipAndTake(skip, take).ToListAsync();

    public Task Create(params TEntity[] entities) => Entities.AddRangeAsync(entities);

    public Task Update(params TEntity[] entities)
    {
        Entities.UpdateRange(entities);
        return Task.CompletedTask;
    }
    public Task Delete(params TEntity[] entities)
    {
        Entities.RemoveRange(entities);
        return Task.CompletedTask;
    }
}
