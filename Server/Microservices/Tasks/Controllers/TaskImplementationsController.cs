using CommonServer.Asp.Filters;
using CommonServer.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks.Services;

namespace Tasks.Controllers;

[ApiController]
[Route("tasks")]
public class TasksImplementationsController: Controller
{
    private readonly TasksService _tasksService;

    public TasksImplementationsController(TasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [Authorize]
    [HttpGet("{id}/implementations/{lang}/{ver}")]
    public Task<IResult> GetTaskImplementation(Guid id, string lang, string ver) =>
        _tasksService.GetImplementation(id, lang, ver, HttpContext.User.GetUserClaims()!);

    [Authorize]
    [HttpGet("{id}/implementations")]
    [ValidationFilter]
    public Task<IResult> GetTaskImplementations(Guid id) =>
        _tasksService.GetImplementations(id, HttpContext.User.GetUserClaims()!);

    [Authorize]
    [HttpDelete("{id}/implementations/{lang}/{ver}")]
    public Task<IResult> DeleteTaskImplementations(Guid id, string lang, string ver) =>
        _tasksService.DeleteImplementation(id, lang, ver, HttpContext.User.GetUserClaims()!);
}
