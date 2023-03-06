using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Services;

namespace Users.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public Task<IResult> Get(Guid id) =>
        _usersService.GetUser(id);
}
