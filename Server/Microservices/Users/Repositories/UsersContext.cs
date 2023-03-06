using CommonServer.Data.Models;
using CommonServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Users.Models;

namespace Users.Repositories;

public class UsersContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshTokenRecord> RefreshTokenRecords { get; set; }
    public DbSet<ConfirmRecord> ConfirmRecords { get; set; }

    public UsersContext(DbContextOptions<UsersContext> options): base(options) { }
}

public class UserContextFactory : IDesignTimeDbContextFactory<UsersContext>
{
    public UsersContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersContext>();

        ConfigurationBuilder builder = new ();
        builder.AddEnvironmentVariables();
        builder.AddDbConnection(builder.Build(), "UsersDB");

        IConfigurationRoot config = builder.Build();
        string connectionString = config.GetConnectionString("UsersDB")!;

        optionsBuilder.UseMySQL(connectionString);

        return new UsersContext(optionsBuilder.Options);
    }
}
