using Microsoft.EntityFrameworkCore;
using CommonServer.Data.Models;
namespace Gateway;

public class OnlyUsersContext: DbContext
{
    public DbSet<User> Users { get; set; }

    public OnlyUsersContext(DbContextOptions<OnlyUsersContext> options): base(options) { }
}
