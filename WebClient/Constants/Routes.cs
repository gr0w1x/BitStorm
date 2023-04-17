using System.Text.Json;
using System.Web;
using Types.Dtos;

namespace WebClient.Constants;

public static class Routes
{
    public const string MainPage = "/main";
    public const string LandingPage = "/hello";

    public const string SignInPage = "/sign-in";
    public const string SignUpPage = "/sign-up";

    public static string UserPage(Guid userId) => $"/users/{userId}";

    public const string TasksSearchPage = "/tasks/search";
    public static string TasksSearchPageWith(GetTasksDto dto) => $"/tasks/search?query={HttpUtility.UrlEncode(JsonSerializer.Serialize(dto))}";

    public static string TaskPage(Guid taskId) => $"/tasks/{taskId}";
    public const string CreateTaskPage = "/tasks/create";
    public static string UpdateTaskPage(Guid taskId) => $"/tasks/{taskId}/update";
    public static string TaskImplementationsPage(Guid taskId) => $"/tasks/{taskId}/implementations";

    public static string SolveTaskPage(Guid taskId) => $"/tasks/{taskId}/solve";
}
