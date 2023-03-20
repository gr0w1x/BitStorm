using CommonServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CommonServer.Data.Repositories;

public abstract class AbstractMySqlContextFactory<T>: IDesignTimeDbContextFactory<T>
    where T: DbContext
{
    protected abstract string ConnectionName { get; }

    protected abstract T MakeContext(DbContextOptions<T> options);

    public T CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<T>();

        ConfigurationBuilder builder = new ();
        builder.AddEnvironmentVariables();
        builder.AddDbConnection(builder.Build(), ConnectionName);

        IConfigurationRoot config = builder.Build();
        string connectionString = config.GetConnectionString(ConnectionName)!;

        optionsBuilder.UseMySQL(connectionString);

        return MakeContext(optionsBuilder.Options);
    }
}
