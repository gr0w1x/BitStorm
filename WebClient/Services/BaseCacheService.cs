namespace WebClient.Services;

public record CachedEntity<TEntity> (TEntity Entity, DateTime Expires);

public abstract class BaseCachedService<TKey, TEntity>
    where TKey: notnull
{
    private readonly IDictionary<TKey, CachedEntity<TEntity>> Entities;

    protected BaseCachedService()
    {
        Entities = new Dictionary<TKey, CachedEntity<TEntity>>();
    }

    protected virtual TimeSpan CacheDuration => TimeSpan.FromMinutes(1);

    protected abstract Task<TEntity> Load(TKey key);

    public async Task<TEntity> Get(TKey key)
    {
        if (Entities.TryGetValue(key, out CachedEntity<TEntity>? cached))
        {
            if (cached.Expires > DateTime.UtcNow)
            {
                return cached.Entity;
            }
        }
        var entity = await Load(key);
        Entities[key] = new CachedEntity<TEntity>(entity, DateTime.UtcNow + CacheDuration);
        return entity;
    }

    public void Set(TKey key, TEntity entity, DateTime Expires)
    {
        Entities[key] = new CachedEntity<TEntity>(entity, Expires);
    }

    public void Set(TKey key, TEntity entity)
    {
        Set(key, entity, DateTime.UtcNow + CacheDuration);
    }

    public void Remove(TKey key)
    {
        Entities.Remove(key);
    }

    public void Clear()
    {
        Entities.Clear();
    }
}
