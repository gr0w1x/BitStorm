using CommonServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Types.Entities;

namespace Tasks.Repositories;

public class TasksContext: AbstractDbContext<TasksContext>
{
    public DbSet<Task_> Tasks { get; set; }
    public DbSet<UserIdRecord> UserIdRecords { get; set; }

    public TasksContext(DbContextOptions<TasksContext> options): base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task_>()
            .Property(nameof(Task_.Tags))
            .HasConversion(new ValueConverter<string[], string>(
                tags => string.Join(", ", tags),
                tags => tags.Split(", ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            ));
    }
}

public class TasksContextFactory : AbstractMySqlContextFactory<TasksContext>
{
    protected override string ConnectionName => "TasksDB";

    protected override TasksContext MakeContext(DbContextOptions<TasksContext> options)
        => new(options);
}
