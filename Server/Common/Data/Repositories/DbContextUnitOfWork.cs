using Microsoft.EntityFrameworkCore;

namespace CommonServer.Data.Repositories;

public class DbContextUnitOfWork<T>: IUnitOfWork, IDisposable
    where T: DbContext
{
    private readonly T _context;

    public DbContextUnitOfWork(T context)
    {
        _context = context;
    }

    public virtual Task Save() => _context.SaveChangesAsync();

    public virtual Task Migrate() => _context.Database.MigrateAsync();

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
