using System.ComponentModel.DataAnnotations;

namespace Types.Entities;

public record SaveImplementationCodeDto
{
    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public string Language { get; set; }
    [Required]
    public string Version { get; set; }

    [Required]
    public string InitialSolution { get; set; }
    [Required]
    public string CompletedSolution { get; set; }
    public string? Preloaded { get; set; }
    [Required]
    public string ExampleTestCases { get; set; }
    [Required]
    public string TestCases { get; set; }
}

public record SolveTaskDto
{
    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public string Language { get; set; }
    [Required]
    public string Version { get; set; }

    [Required]
    public string Solution { get; set; }
}

public static class ExecuteCodeType
{
    public const string SaveImplementationCode = "save-implementation";
    public const string SolveTask = "solve-task";
}
