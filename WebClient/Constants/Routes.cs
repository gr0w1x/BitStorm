namespace WebClient.Constants;

public static class Routes
{
    public const string SignInPage = "/sign-in";
    public const string SignUpPage = "/sign-up";
    public const string LandingPage = "/hello";
    public const string CreateTaskPage = "/tasks/create";
    public const string MainPage = "/main";

    public static string UserPage(Guid userId) => $"/users/{userId}";
    public static string TaskPage(Guid taskId) => $"/tasks/{taskId}";
}
