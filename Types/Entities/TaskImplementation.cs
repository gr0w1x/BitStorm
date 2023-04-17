using System.ComponentModel.DataAnnotations;

namespace Types.Entities;

public interface ITaskImplementation: ICreated, IUpdated
{
    Guid TaskId { get; }
    string Language { get; }
    string Version { get; }

    string? Details { get; set; }

    string InitialSolution { get; set; }
    string CompletedSolution { get; set; }

    string? PreloadedCode { get; set; }

    string ExampleTests { get; set; }
    string Tests { get; set; }
}

public record TaskImplementation: ITaskImplementation
{
    public Guid TaskId { get; }
    public string Language { get; set; }
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

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
