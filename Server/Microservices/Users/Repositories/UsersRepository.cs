using CommonServer.Data.Repositories;
using CommonServer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Users.Repositories;

public interface IUsersRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailOrUsername(string email, string username);
}

public class UsersRepository:
    DbContextRepository<User, UsersContext, Guid>,
    IUsersRepository
{
    protected override DbSet<User> GetEntitiesBy(UsersContext context) => context.Users;

    public Task<User?> GetByEmailOrUsername(string email, string username) =>
        Entities.FirstOrDefaultAsync(
            user => user.Email == email || user.Username == username
        );

    public UsersRepository(UsersContext context) : base(context) { }
}
