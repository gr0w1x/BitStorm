using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Types.Entities;

public enum TaskVisibility
{
    Private,
    Public
}

public interface ITask: IHasId
{
    string Title { get; set; }
    string? Description { get; set; }

    Guid AuthorId { get; }

    List<UserIdRecord> Likes { get; set; }

    int Level { get; set; }
    bool Beta { get; set; }

    List<TaskTag> Tags { get; set; }

    TaskVisibility Visibility { get; set; }

    ICollection<TaskImplementation> Implementations { get; set; }
}

public interface ITaskTag: IHasId<string>
{
    List<Task_> Tasks { get; set; }
}

public interface ITaskImplementation
{
    Guid TaskId { get; }
    string Language { get; }
    string Version { get; }

    string InitialSolution { get; set; }
    string CompleteSolution { get; set; }

    string PreloadedCode { get; set; }

    string ExampleTests { get; set; }
    string Tests { get; set; }
}

public static class TaskConstants
{
    public const int MinTitleLength = 1;
    public const int MaxTitleLength = 256;

    public const int MaxDescriptionLength = 4096;

    public const int MaxTags = 16;

    public const int MaxTagLength = 256;

    public const int MaxLevel = 9;
}

public record Task_: ITask, ICreated, IUpdated
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid AuthorId { get; set; }

    [JsonIgnore]
    public List<UserIdRecord> Likes { get; set; }

    [Required]
    [MinLength(TaskConstants.MinTitleLength)]
    [MaxLength(TaskConstants.MaxTitleLength)]
    public string Title { get; set; }

    [MaxLength(TaskConstants.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Range(1, TaskConstants.MaxLevel)]
    public int Level { get; set; }

    [Required]
    public bool Beta { get; set; }

    public List<TaskTag> Tags { get; set; } = new List<TaskTag>();

    public TaskVisibility Visibility { get; set; } = TaskVisibility.Private;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<TaskImplementation> Implementations { get; set; }
}

public record TaskTag: ITaskTag
{
    [Key]
    public string Id { get; set; }

    [JsonIgnore]
    public List<Task_> Tasks { get; set; } = new List<Task_>();
}

public record TaskImplementation: ITaskImplementation
{
    public Guid TaskId { get; }
    public string Language { get; set; }
    public string Version { get; set; }

    [Required]
    public string InitialSolution { get; set; }
    [Required]
    public string CompleteSolution { get; set; }
    [Required]
    public string PreloadedCode { get; set; }
    [Required]
    public string ExampleTests { get; set; }
    [Required]
    public string Tests { get; set; }
}
