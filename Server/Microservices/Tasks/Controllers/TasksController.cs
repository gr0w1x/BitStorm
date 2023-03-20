using CommonServer.Asp.Filters;
using CommonServer.Utils.Extensions;
using Gateway.Services;
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

    [AllowAnonymous]
    [HttpGet("public/{id}")]
    public Task<IResult> GetTask(Guid id) =>
        _tasksService.GetTask(id, HttpContext.GetJwt());

    [Authorize]
    [HttpPost("create")]
    [ValidationFilter]
    public Task<IResult> CreateTask([FromBody] CreateTaskDto dto) =>
        _tasksService.CreateTask(dto, HttpContext.GetJwt()!);
}