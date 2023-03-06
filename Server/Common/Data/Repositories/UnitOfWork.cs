namespace CommonServer.Data.Repositories;

public interface IUnitOfWork
{
    Task Save();
    Task Migrate();
}
