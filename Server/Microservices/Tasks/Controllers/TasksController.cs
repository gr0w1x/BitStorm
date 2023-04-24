using CommonServer.Asp.Filters;
using CommonServer.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks.Services;
using Types.Dtos;

namespace Tasks.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController: Controller
{
    private readonly TasksService _tasksService;

    public TasksController(TasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [Authorize]
    [AllowAnonymous]
    [HttpGet("public/{id}")]
    public Task<IResult> GetTask(Guid id) =>
        _tasksService.GetTask(id, HttpContext.User.GetUserClaims());

    [Authorize]
    [AllowAnonymous]
    [HttpGet("public/info")]
    [ValidationFilter]
    public Task<IResult> GetTasksInfo([FromQuery] GetTasksInfoDto dto) =>
        _tasksService.GetTasksInfo(dto, HttpContext.User.GetUserClaims());

    [Authorize]
    [AllowAnonymous]
    [HttpGet("public/search")]
    [ValidationFilter]
    public Task<IResult> GetTasks([FromQuery] GetTasksDto dto) =>
        _tasksService.GetTasks(dto, HttpContext.User.GetUserClaims());

    [Authorize]
    [HttpPost("create")]
    [ValidationFilter]
    public Task<IResult> CreateTask([FromBody] CreateTaskDto dto) =>
        _tasksService.CreateTask(dto, HttpContext.User.GetUserClaims()!);

    [Authorize]
    [HttpPost("{id}")]
    [ValidationFilter]
    public Task<IResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto) =>
        _tasksService.UpdateTask(id, dto, HttpContext.User.GetUserClaims()!);

    [Authorize]
    [HttpDelete("{id}")]
    public Task<IResult> DeleteTask(Guid id) =>
        _tasksService.DeleteTask(id, HttpContext.User.GetUserClaims()!);

    [Authorize]
    [HttpPost("{id}/approve")]
    [ValidationFilter]
    public Task<IResult> ApproveTask(Guid id, [FromBody] ApproveTaskDto dto) =>
        _tasksService.ApproveTask(id, dto, HttpContext.User.GetUserClaims()!);

    [Authorize]
    [HttpPost("{id}/publish")]
    [ValidationFilter]
    public Task<IResult> PublishTask(Guid id) =>
        _tasksService.PublishTask(id, HttpContext.User.GetUserClaims()!);
}
