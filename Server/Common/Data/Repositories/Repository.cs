namespace CommonServer.Data.Repositories;

public interface IRepository<TEntity, TId>
{
    Task<TEntity?> GetById(TId id);
    Task<IEnumerable<TEntity>> Select(int? skip = null, int? take = null);
    Task Create(params TEntity[] entities);
    Task Update(params TEntity[] entities);
    Task Delete(params TEntity[] entities);
}
