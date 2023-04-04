using System.ComponentModel.DataAnnotations;
using Types.Entities;

namespace Types.Dtos;

public record TasksInfoDto
{
    public int Total { get; set; }
    public Dictionary<string, int> Tags { get; set; }
}

public record GetTasksInfoDto
{
    public string? Query { get; set; }
    public StatusOptions Status { get; set; } = StatusOptions.All;
    public string[]? Languages { get; set; }
    public int[]? Levels { get; set; }
    public string[]? Tags { get; set; }

    [Flags]
    public enum StatusOptions
    {
        None         = 0,
        OnlyBeta     = 1 << 0,
        OnlyApproved = 1 << 1,
        All          = OnlyBeta | OnlyApproved
    }
}

public record GetTasksDto: GetTasksInfoDto
{
    public SortStrategy? Sort { get; set; }
    [Required]
    public int Skip { get; set; }
    [Required]
    public int Take { get; set; }

    public enum SortStrategy
    {
        LastUpdated,
        Likes,
        Level,
        Name
    }
}

public record CreateTaskDto
{
    [Required(ErrorMessage = "title required")]
    [MinLength(TaskConstants.MinTitleLength, ErrorMessage = "required at least 1 character")]
    [MaxLength(TaskConstants.MaxTitleLength, ErrorMessage = "too long title (256 max)")]
    public string Title { get; set; }

    [MaxLength(TaskConstants.MaxDescriptionLength, ErrorMessage = "too long title (4096 max)")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "suggested level required")]
    [Range(1, TaskConstants.MaxLevel, ErrorMessage = "possible task difficulty level values: 1-9")]
    public int SuggestedLevel { get; set; } = 9;

    [Required(ErrorMessage = "tags required")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [Required(ErrorMessage = "task visibility required")]
    public TaskVisibility Visibility { get; set; } = TaskVisibility.Private;
}

public record EditTaskDto: CreateTaskDto
{
    [Required(ErrorMessage = "task id required")]
    public Guid TaskId { get; }
}
