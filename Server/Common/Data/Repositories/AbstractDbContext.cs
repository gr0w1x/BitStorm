using Microsoft.EntityFrameworkCore;
using Types.Entities;

namespace CommonServer.Data.Repositories;

public class AbstractDbContext<T>: DbContext
    where T: DbContext
{
    public AbstractDbContext(DbContextOptions<T> options): base(options) { }

    public override int SaveChanges()
    {
        foreach (var change in ChangeTracker.Entries())
        {
            if (change.Entity is ICreated created && change.State == EntityState.Added)
            {
                created.CreatedAt = DateTimeOffset.UtcNow;
            }
            else if (change.Entity is IUpdated updated && change.State == EntityState.Modified)
            {
                updated.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken _)
        => Task.FromResult(SaveChanges());
}
