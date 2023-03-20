namespace CommonServer.Data.Messages;

public record ExecuteTestsResultMessage
{
    public int Passed { get; init; }
    public int Failed { get; init; }
}

public record ExecuteCodeResultMessage
{
    public int ExitStatus { get; init; }
    public TimeSpan Time { get; init; }
    public string? Details { get; init; }
    public ExecuteTestsResultMessage? Tests { get; init; }
}
