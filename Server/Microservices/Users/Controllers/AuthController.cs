using CommonServer.Asp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Types.Dtos;
using Users.Services;

namespace Users.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    public Task<IResult> SignIn([FromBody] SignInDto body)
        => _authService.SignIn(body.EmailOrUsername, body.Password);

    [AllowAnonymous]
    [HttpPost("sign-up")]
    [ValidationFilter]
    public Task<IResult> SignUp([FromBody] SignUpDto body)
        => _authService.SignUp(body.Email, body.Username, body.Password);

    [AllowAnonymous]
    [HttpPost("confirm")]
    public Task<IResult> Confirm([FromBody] ConfirmDto body)
        => _authService.Confirm(body.code);

    [AllowAnonymous]
    [HttpPost("refresh")]
    public Task<IResult> Refresh([FromBody] string refreshToken)
        => _authService.Refresh(refreshToken);
}
