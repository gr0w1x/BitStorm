namespace CommonServer.Constants;

public static class RabbitMqQueries
{
    public const string MailerQuery = "mailer";
    public const string TasksQuery = "tasks";
    public static string ExecutorQuery(string language, string version) => $"executor-{language}-{version}";
}
