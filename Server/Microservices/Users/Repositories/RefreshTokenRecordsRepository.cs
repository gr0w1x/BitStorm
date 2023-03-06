using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users.Repositories;

public interface IRefreshTokenRecordsRepository: IRepository<RefreshTokenRecord, string>
{
    Task DeleteExpired();
}

public class RefreshTokenRecordsRepository:
    DbContextRepository<RefreshTokenRecord, UsersContext, string>,
    IRefreshTokenRecordsRepository
{
    public RefreshTokenRecordsRepository(UsersContext context) : base(context) { }

    protected override DbSet<RefreshTokenRecord> GetEntitiesBy(UsersContext context) => context.RefreshTokenRecords;

    public Task DeleteExpired()
    {
        Entities.RemoveRange(
            Entities.Where(record => record.Expired < DateTimeOffset.Now)
        );
        return Task.CompletedTask;
    }
}
