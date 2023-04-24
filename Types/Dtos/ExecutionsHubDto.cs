using System.ComponentModel.DataAnnotations;

namespace Types.Dtos;

public record SaveImplementationCodeDto
{
    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public string Language { get; set; }
    [Required]
    public string Version { get; set; }

    public string? Details { get; set; }
    [Required]
    public string InitialSolution { get; set; }
    [Required]
    public string CompletedSolution { get; set; }
    public string? PreloadedCode { get; set; }
    [Required]
    public string ExampleTests { get; set; }
    [Required]
    public string Tests { get; set; }
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
