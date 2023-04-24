using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Types.Entities;

public interface ITaskImplementation: ICreated, IUpdated
{
    Guid TaskId { get; }
    string Language { get; }
    string Version { get; }

    string? Details { get; set; }
    string InitialSolution { get; set; }
    string ExampleTests { get; set; }

    string CompletedSolution { get; set; }
    string Tests { get; set; }
    string? PreloadedCode { get; set; }
}

public record TaskImplementation: ITaskImplementation
{
    public Guid TaskId { get; set; }
    public string Language { get; set; }
    public string Version { get; set; }

    public string? Details { get; set; }
    [Required]
    public string InitialSolution { get; set; }

    [Required]
    public string ExampleTests { get; set; }

    [Required]
    [JsonIgnore]
    public string CompletedSolution { get; set; }
    [JsonIgnore]
    public string? PreloadedCode { get; set; }
    [Required]
    [JsonIgnore]
    public string Tests { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
