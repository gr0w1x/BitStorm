using System.Net;
using CommonServer.Data.Repositories;
using Tasks.Repositories;
using Types.Constants.Errors;
using Types.Dtos;
using Types.Entities;

namespace Tasks.Services;

public class TasksService
{
    private readonly IUnitOfWork _work;
    private readonly ITasksRepository _tasksRepository;
    private readonly ITaskImplementationsRepository _tasksImplementationsRepository;

    public TasksService(
        IUnitOfWork work,
        ITasksRepository tasksRepository,
        ITaskImplementationsRepository taskImplementationsRepository
    )
    {
        _work = work;
        _tasksRepository = tasksRepository;
        _tasksImplementationsRepository = taskImplementationsRepository;
    }

    public async Task<IResult> GetTask(Guid id, UserClaims? userClaims)
    {
        var task = await _tasksRepository.GetById(id);
        if (task?.IsVisible(userClaims) == true)
        {
            return Results.Ok(task);
        }
        return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
    }

    public async Task<IResult> GetTasksInfo(GetTasksInfoDto dto, UserClaims? userClaims)
        => Results.Ok(await _tasksRepository.GetTasksInfo(
            dto,
            userClaims != null && ((userClaims.Roles & UserRoles.Admin) != UserRoles.None),
            userClaims?.UserId
        ));

    public async Task<IResult> GetTasks(GetTasksDto dto, UserClaims? userClaims)
        => Results.Ok(await _tasksRepository.GetTasks(
            dto,
            userClaims != null && ((userClaims.Roles & UserRoles.Admin) != UserRoles.None),
            userClaims?.UserId
        ));

    public async Task<IResult> CreateTask(CreateTaskDto dto, UserClaims userClaims)
    {
        var existedTaskWithTitle = await _tasksRepository.GetByTitle(dto.Title);

        if (existedTaskWithTitle != null)
        {
            return Results.BadRequest(
                new ErrorDto("task with given title already exists",
                CommonErrors.ValidationError
            ));
        }

        var task = new Task_()
        {
            AuthorId = userClaims.UserId,
            Title = dto.Title,
            Description = dto.Description,
            Level = dto.SuggestedLevel ?? 9,
            Beta = true,
            Tags = (dto.Tags ?? Array.Empty<string>()).Where(str => !string.IsNullOrWhiteSpace(str)).Select(tag =>
                new TaskTag()
                {
                    Id = tag,
                }
            ).ToList(),
            Likes = 0,
            Visibility = dto.Visibility
        };

        await _tasksRepository.Create(task);
        await _work.Save();

        return Results.Ok(task);
    }

    public async Task<IResult> UpdateTask(Guid id, UpdateTaskDto dto, UserClaims userClaims)
    {
        var task = await _tasksRepository.GetById(id);

        if (task?.IsVisible(userClaims) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanUpdate(userClaims))
        {
            return Results.Json(
                new ErrorDto("no access to update this task", CommonErrors.ForbiddenError),
                statusCode: (int)HttpStatusCode.Forbidden
            );
        }

        if (dto.Title != null)
        {
            var existedTaskWithTitle = await _tasksRepository.GetByTitle(dto.Title);

            if (existedTaskWithTitle != null && existedTaskWithTitle.Id != id)
            {
                return Results.BadRequest(
                    new ErrorDto("task with given title already exists",
                    CommonErrors.ValidationError
                ));
            }

            task.Title = dto.Title;
        }
        if (dto.Description != null)
        {
            task.Description = dto.Description;
        }
        if (dto.Level != null)
        {
            task.Level = dto.Level.Value;
            task.Beta = true;
        }

        if (dto.Tags != null)
        {
            task.Tags = dto.Tags.Where(str => !string.IsNullOrWhiteSpace(str)).Select(tag =>
                new TaskTag()
                {
                    Id = tag,
                }
            ).ToList();
        }

        await _tasksRepository.Update(task);
        await _work.Save();

        return Results.Ok(task);
    }

    public async Task<IResult> DeleteTask(Guid id, UserClaims userClaims)
    {
        var task = await _tasksRepository.GetById(id);

        if (task?.IsVisible(userClaims) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanDelete(userClaims))
        {
            return Results.Json(
                new ErrorDto("no access to delete this task", CommonErrors.ForbiddenError),
                statusCode: (int)HttpStatusCode.Forbidden
            );
        }

        await _tasksRepository.Delete(task);
        await _work.Save();

        return Results.Ok();
    }

    public async Task<IResult> ApproveTask(Guid id, ApproveTaskDto dto, UserClaims userClaims)
    {
        var task = await _tasksRepository.GetById(id);

        if (task?.IsVisible(userClaims) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanApprove(userClaims))
        {
            return Results.Json(
                new ErrorDto("no access to approve this task", CommonErrors.ForbiddenError),
                statusCode: (int)HttpStatusCode.Forbidden
            );
        }

        if (!task.Beta)
        {
            return Results.Json(
                new ErrorDto("already approved", TasksErrors.AlreadyApproved),
                statusCode: (int)HttpStatusCode.BadRequest
            );
        }

        if (dto.Level != null)
        {
            task.Level = dto.Level.Value;
        }
        task.Beta = false;

        await _tasksRepository.Update(task);
        await _work.Save();

        return Results.Ok(task);
    }

    public async Task<IResult> PublishTask(Guid id, UserClaims userClaims)
    {
        var task = await _tasksRepository.GetById(id);

        if (task?.IsVisible(userClaims) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanPublish(userClaims))
        {
            return Results.Json(
                new ErrorDto("no access to publish this task", CommonErrors.ForbiddenError),
                statusCode: (int)HttpStatusCode.Forbidden
            );
        }

        if (task.Visibility == TaskVisibility.Public)
        {
            return Results.Json(
                new ErrorDto("already published", TasksErrors.AlreadyPublished),
                statusCode: (int)HttpStatusCode.BadRequest
            );
        }

        task.Visibility = TaskVisibility.Public;

        await _tasksRepository.Update(task);
        await _work.Save();

        return Results.Ok(task);
    }

    public async Task<IResult> GetImplementation(
        Guid taskId,
        string language,
        string version,
        UserClaims userClaims
    )
    {
        var task = await _tasksRepository.GetById(taskId);

        if (task?.IsVisible(userClaims) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.IsImplementationsVisible(userClaims))
        {
            return Results.Json(
                new ErrorDto("no access to task implementations", CommonErrors.ForbiddenError),
                statusCode: (int)HttpStatusCode.Forbidden
            );
        }

        var implementation = await _tasksImplementationsRepository.Get(taskId, language, version);

        if (implementation == null)
        {
            return Results.NotFound(new ErrorDto("no task implementation found", CommonErrors.NotFoundError));
        }

        return Results.Ok((TaskImplementationWithSecretDto)implementation);
    }

    public async Task<IResult> GetImplementations(Guid taskId, UserClaims userClaims)
    {
        var task = await _tasksRepository.GetById(taskId);

        if (task?.IsVisible(userClaims) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.IsImplementationsVisible(userClaims))
        {
            return Results.Json(
                new ErrorDto("no access to task implementations", CommonErrors.ForbiddenError),
                statusCode: (int)HttpStatusCode.Forbidden
            );
        }

        var implementations = await _tasksImplementationsRepository.GetByTask(taskId);

        return Results.Ok(implementations.Select(impl => (TaskImplementationWithSecretDto)impl));
    }

    public async Task<(TaskImplementationWithSecretDto? Implementation, ErrorDto? Error)> SaveTaskImplementation(SaveImplementationCodeDto dto, UserClaims userClaims)
    {
        var task = await _tasksRepository.GetById(dto.TaskId);

        if (task?.IsVisible(userClaims) != true)
        {
            return (null, new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanUpdate(userClaims))
        {
            return (null, new ErrorDto("no access to update this task", CommonErrors.ForbiddenError));
        }

        var implementation =
            task
                .Implementations
                .FirstOrDefault(impl => impl.Language == dto.Language && impl.Version == dto.Version)
            ?? new TaskImplementation()
            {
                TaskId = dto.TaskId,
                Language = dto.Language,
                Version = dto.Version
            };

        implementation.Details = dto.Details;
        implementation.InitialSolution = dto.InitialSolution;
        implementation.CompletedSolution = dto.CompletedSolution;
        implementation.PreloadedCode = dto.PreloadedCode;
        implementation.ExampleTests = dto.ExampleTests;
        implementation.Tests = dto.Tests;

        if (!task.Implementations.Contains(implementation))
        {
            task.Implementations.Add(implementation);
        }

        await _tasksRepository.Update(task);

        await _work.Save();

        return ((TaskImplementationWithSecretDto)implementation, null);
    }

    public async Task<IResult> DeleteImplementation(
        Guid taskId,
        string language,
        string version,
        UserClaims userClaims
    )
    {
        var task = await _tasksRepository.GetById(taskId);

        if (task?.IsVisible(userClaims) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.IsImplementationsVisible(userClaims))
        {
            return Results.Json(
                new ErrorDto("no access to task implementations", CommonErrors.ForbiddenError),
                statusCode: (int)HttpStatusCode.Forbidden
            );
        }

        await _tasksImplementationsRepository.Delete(taskId, language, version);
        await _work.Save();

        return Results.Ok();
    }
}
