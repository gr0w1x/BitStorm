namespace Types.Dtos;

public record LanguageDto
{
    public string Code { get; init; }
    public string Name { get; init; }
    public VersionDto[] Versions { get; init; }
    public VersionDto DefaultVersion { get; init; }
}

public record VersionDto
{
    public string Language { get; init; }
    public string Version { get; init; }
    public TimeSpan ExecutionTimeout { get; init; }
    public int RamSizeLimitMb { get; init; }
    public string ExecutorQueue { get; init; }
}
