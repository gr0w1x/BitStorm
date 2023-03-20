using System.Net;
using CommonServer.Data.Repositories;
using Tasks.Repositories;
using Types.Dtos;
using Types.Entities;

namespace Tasks.Services;

public class TasksService
{
    public IUnitOfWork _work;
    public ITasksRepository _tasksRepository;

    public TasksService(IUnitOfWork work, ITasksRepository tasksRepository)
    {
        _work = work;
        _tasksRepository = tasksRepository;
    }

    public async Task<IResult> GetTask(Guid id, AccessJwtTokenContent? jwt)
    {
        var task = await _tasksRepository.GetById(id);
        if (
            task != null &&
            (
                task.Visibility == TaskVisibility.Public ||
                (
                    task.Visibility == TaskVisibility.Private && jwt != null && (
                        task.AuthorId == jwt.UserId ||
                        jwt.Roles == UserRoles.Admin
                    )
                )
            )
        )
        {
            return Results.Ok(new TaskDto()
            {
                Task = task,
                Likes = task.Likes.Count,
            });
        }
        return Results.NotFound(new ErrorDto("no task found", HttpStatusCode.NotFound));
    }

    public async Task<IResult> CreateTask(CreateTaskDto dto, AccessJwtTokenContent jwt)
    {
        var existedTaskWithTitle = await _tasksRepository.GetByTitle(dto.Title);

        if (existedTaskWithTitle != null)
        {
            return Results.BadRequest(new ErrorDto("task with given title already exists", HttpStatusCode.BadRequest));
        }

        var task = new Task_()
        {
            AuthorId = jwt.UserId,
            Title = dto.Title,
            Description = dto.Description,
            Level = dto.SuggestedLevel,
            Beta = true,
            Tags = dto.Tags,
            Likes = new List<UserIdRecord>(),
            Visibility = dto.Visibility
        };

        await _tasksRepository.Create(task);
        await _work.Save();

        return Results.Ok(new TaskDto()
        {
            Task = task,
            Likes = task.Likes.Count,
        });
    }
}
