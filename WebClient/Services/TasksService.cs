using System.Text;
using System.Text.Json;
using Types.Dtos;
using Types.Entities;
using WebClient.Extensions;

namespace WebClient.Services;

public class TasksService: BaseCachedService<Guid, Task_?>
{
    private readonly ApiClient _apiClient;

    public TasksService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    protected override async Task<Task_?> Load(Guid taskId) =>
        await _apiClient.TrySendAndRecieve<Task_>(new ApiMessage()
        {
            RequestUri = new Uri($"/api/public/tasks/{taskId}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
            OptionalAuthorization = true,
        });

    public Task<TasksInfoDto> GetTasksInfo(GetTasksInfoDto dto) =>
        _apiClient.SendAndRecieve<TasksInfoDto>(new ApiMessage()
        {
            RequestUri = new Uri($"api/public/tasks/info?{dto.ToQueryString()}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
            OptionalAuthorization = true
        });

    public async Task<IEnumerable<Task_>> GetTasks(GetTasksDto dto)
    {
        var tasks = await _apiClient.SendAndRecieve<IEnumerable<Task_>>(new ApiMessage()
        {
            RequestUri = new Uri($"api/public/tasks/search?{dto.ToQueryString()}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
            OptionalAuthorization = true
        });
        foreach (var task in tasks)
        {
            Set(task.Id, task);
        }
        return tasks;
    }

    public async Task<Task_> CreateTask(CreateTaskDto dto)
    {
        var task = await _apiClient.SendAndRecieve<Task_>(new ApiMessage()
        {
            RequestUri = new Uri("api/tasks/create", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
            NeedAuthorization = true
        });
        Set(task.Id, task);
        return task;
    }

    public async Task<Task_> UpdateTask(Guid taskId, UpdateTaskDto dto)
    {
        var task = await _apiClient.SendAndRecieve<Task_>(new ApiMessage()
        {
            RequestUri = new Uri($"api/tasks/{taskId}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
            NeedAuthorization = true
        });
        Set(task.Id, task);
        return task;
    }

    public async Task DeleteTask(Guid taskId)
    {
        await _apiClient.SendApiAsync(new ApiMessage()
        {
            RequestUri = new Uri($"api/tasks/{taskId}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Delete,
            NeedAuthorization = true
        });
        Remove(taskId);
    }

    public async Task<Task_> PublishTask(Guid taskId)
    {
        var task = await _apiClient.SendAndRecieve<Task_>(new ApiMessage()
        {
            RequestUri = new Uri($"api/tasks/{taskId}/publish", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            NeedAuthorization = true
        });
        Set(task.Id, task);
        return task;
    }

    public async Task<Task_> ApproveTask(Guid taskId, ApproveTaskDto dto)
    {
        var task = await _apiClient.SendAndRecieve<Task_>(new ApiMessage()
        {
            RequestUri = new Uri($"api/tasks/{taskId}/approve", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
            NeedAuthorization = true
        });
        Set(task.Id, task);
        return task;
    }
}
