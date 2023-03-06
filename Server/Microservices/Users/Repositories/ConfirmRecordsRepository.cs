using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users.Repositories;

public interface IConfirmRecordsRepository: IRepository<ConfirmRecord, Guid>
{
    Task DeleteExpired();
}

public class ConfirmRecordsRepository:
    DbContextRepository<ConfirmRecord, UsersContext, Guid>,
    IConfirmRecordsRepository
{
    public ConfirmRecordsRepository(UsersContext context) : base(context) { }

    protected override DbSet<ConfirmRecord> GetEntitiesBy(UsersContext context) => context.ConfirmRecords;

    public Task DeleteExpired()
    {
        Entities.RemoveRange(
            Entities.Where(record => record.Expired < DateTimeOffset.UtcNow)
        );
        return Task.CompletedTask;
    }
}
