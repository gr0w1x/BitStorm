using System.Text;
using System.Text.Json;
using Types.Dtos;
using Types.Entities;

namespace WebClient.Services;

public class TasksService: BaseApiService
{
    public TasksService(ApiClient apiClient): base(apiClient) { }

    public async Task<Task_?> GetTask(Guid taskId) =>
        await TrySendAndRecieve<Task_>(new ApiMessage()
        {
            RequestUri = new Uri($"/api/public/tasks/{taskId}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
            OptionalAuthorization = true,
        });

    public async Task<Task_> CreateTask(CreateTaskDto dto) =>
        await SendAndRecieve<Task_>(new ApiMessage()
        {
            RequestUri = new Uri("api/tasks/create", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"),
            NeedAuthorization = true
        });
}
