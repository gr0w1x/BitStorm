namespace CommonServer.Constants;

public static class RabbitMqQueries
{
    public static string ExecutorQuery(string language, string version) => $"executor-{language}-{version}";
}
