using CommonServer.Data.Models;
using CommonServer.Data.Repositories;
using CommonServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Users.Models;

namespace Users.Repositories;

public class UsersContext: AbstractDbContext<UsersContext>
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshTokenRecord> RefreshTokenRecords { get; set; }
    public DbSet<ConfirmRecord> ConfirmRecords { get; set; }

    public UsersContext(DbContextOptions<UsersContext> options): base(options) { }
}

public class UserContextFactory : AbstractMySqlContextFactory<UsersContext>
{
    protected override string ConnectionName => "UsersDB";

    protected override UsersContext MakeContext(DbContextOptions<UsersContext> options)
        => new(options);
}
