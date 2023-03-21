using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Types.Entities;

namespace Tasks.Repositories;

public class TasksContext: AbstractDbContext<TasksContext>
{
    public DbSet<Task_> Tasks { get; set; }
    public DbSet<TaskTag> Tags { get; set; }

    public DbSet<UserIdRecord> UserIdRecords { get; set; }

    public TasksContext(DbContextOptions<TasksContext> options): base(options) { }
}

public class TasksContextFactory : AbstractMySqlContextFactory<TasksContext>
{
    protected override string ConnectionName => "TasksDB";

    protected override TasksContext MakeContext(DbContextOptions<TasksContext> options)
        => new(options);
}
