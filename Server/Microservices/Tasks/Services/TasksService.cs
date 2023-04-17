using System.Net;
using CommonServer.Data.Repositories;
using Tasks.Repositories;
using Types.Constants.Errors;
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
        if (task?.IsVisible((PublicUser?)jwt) == true)
        {
            return Results.Ok(task);
        }
        return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
    }

    public async Task<IResult> GetTasksInfo(GetTasksInfoDto dto, AccessJwtTokenContent? jwt)
        => Results.Ok(await _tasksRepository.GetTasksInfo(
            dto,
            jwt != null && ((jwt.Roles & UserRoles.Admin) != UserRoles.None),
            jwt?.UserId
        ));

    public async Task<IResult> GetTasks(GetTasksDto dto, AccessJwtTokenContent? jwt)
        => Results.Ok(await _tasksRepository.GetTasks(
            dto,
            jwt != null && ((jwt.Roles & UserRoles.Admin) != UserRoles.None),
            jwt?.UserId
        ));

    public async Task<IResult> CreateTask(CreateTaskDto dto, AccessJwtTokenContent jwt)
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
            AuthorId = jwt.UserId,
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

    public async Task<IResult> UpdateTask(Guid id, UpdateTaskDto dto, AccessJwtTokenContent jwt)
    {
        var task = await _tasksRepository.GetById(id);

        var user = ((PublicUser)jwt)!;

        if (task?.IsVisible(user) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanUpdate(user))
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

    public async Task<IResult> DeleteTask(Guid id, AccessJwtTokenContent jwt)
    {
        var task = await _tasksRepository.GetById(id);

        var user = ((PublicUser)jwt)!;

        if (task?.IsVisible(user) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanDelete(user))
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

    public async Task<IResult> ApproveTask(Guid id, ApproveTaskDto dto, AccessJwtTokenContent jwt)
    {
        var task = await _tasksRepository.GetById(id);

        var user = ((PublicUser)jwt)!;

        if (task?.IsVisible(user) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanApprove(user))
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

    public async Task<IResult> PublishTask(Guid id, AccessJwtTokenContent jwt)
    {
        var task = await _tasksRepository.GetById(id);

        var user = ((PublicUser)jwt)!;

        if (task?.IsVisible(user) != true)
        {
            return Results.NotFound(new ErrorDto("no task found", CommonErrors.NotFoundError));
        }

        if (!task.CanPublish(user))
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
}
