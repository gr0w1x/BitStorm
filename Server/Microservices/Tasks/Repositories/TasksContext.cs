using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Types.Entities;

namespace Tasks.Repositories;

public class TasksContext: AbstractDbContext<TasksContext>
{
    public DbSet<Task_> Tasks { get; set; }
    public DbSet<TaskTag> Tags { get; set; }
    public DbSet<TaskImplementation> Implementations { get; set; }

    public DbSet<UserIdRecord> UserIdRecords { get; set; }

    public TasksContext(DbContextOptions<TasksContext> options): base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
            .Entity<TaskImplementation>()
            .HasKey(implement => new { implement.TaskId, implement.Language, implement.Version });
    }
}

public class TasksContextFactory : AbstractMySqlContextFactory<TasksContext>
{
    protected override string ConnectionName => "TasksDB";

    protected override TasksContext MakeContext(DbContextOptions<TasksContext> options)
        => new(options);
}
