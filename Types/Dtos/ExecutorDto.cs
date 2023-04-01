namespace Types.Dtos;

public record ExecuteCodeDto(string Preloaded, string Solution, string Tests);

public record ExecuteTests
{
    public int Passed { get; init; }
    public int Failed { get; init; }
}

public record ExecuteCodeResultDto
{
    public int ExitStatus { get; init; }
    public TimeSpan Time { get; init; }
    public string? Details { get; init; }
    public ExecuteTests? Tests { get; init; }
}
