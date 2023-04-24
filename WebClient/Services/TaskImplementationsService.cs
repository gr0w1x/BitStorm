using Types.Dtos;
using Types.Entities;

namespace WebClient.Services;

public record TaskImplementationId(Guid TaskId, string Language, string Version);

public class TaskImplementationsService: BaseCachedService<TaskImplementationId, TaskImplementation?>
{
    private readonly ApiClient _apiClient;

    public TaskImplementationsService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    protected static TaskImplementation? ToTaskImplementation(TaskImplementationWithSecretDto? dto) =>
        dto == null
            ? null
            : dto.TaskImplementation with
            {
                CompletedSolution = dto.CompletedSolution,
                PreloadedCode = dto.PreloadedCode,
                Tests = dto.Tests
            };

    protected override async Task<TaskImplementation?> Load(TaskImplementationId key)
    {
        var implement = await _apiClient.TrySendAndRecieve<TaskImplementationWithSecretDto>(new ApiMessage()
        {
            RequestUri = new Uri($"/api/tasks/{key.TaskId}/implementations/{key.Language}/{key.Version}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
            NeedAuthorization = true,
        });

        return ToTaskImplementation(implement);
    }

    public async Task<List<TaskImplementation>> GetImplementations(Guid taskId)
    {
        var implements = (await _apiClient.SendAndRecieve<List<TaskImplementationWithSecretDto>>(new ApiMessage()
        {
            RequestUri = new Uri($"/api/tasks/{taskId}/implementations", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Get,
            OptionalAuthorization = true,
        }))
            .Select(ToTaskImplementation)
            .Cast<TaskImplementation>()
            .ToList();

        foreach (var implement in implements)
        {
            var key = new TaskImplementationId(
                taskId,
                implement.Language,
                implement.Version
            );
            Set(key, implement);
        }

        return implements;
    }

    public async Task Delete(TaskImplementationId id)
    {
        await _apiClient.SendApiAsync(new ApiMessage()
        {
            RequestUri = new Uri($"api/tasks/{id.TaskId}/implementations/{id.Language}/{id.Version}", UriKind.RelativeOrAbsolute),
            Method = HttpMethod.Delete,
            NeedAuthorization = true
        });
        Remove(id);
    }
}
