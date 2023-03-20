using System.ComponentModel.DataAnnotations;
using Types.Entities;

namespace Types.Dtos;

public record TaskDto
{
    public Task_ Task { get; set; }
    public int Likes  { get; set; }
}

public record CreateTaskDto
{
    [Required]
    [MinLength(TaskConstants.MinTitleLength, ErrorMessage = "required at least 1 character")]
    [MaxLength(TaskConstants.MaxTitleLength, ErrorMessage = "too long title (256 max)")]
    public string Title { get; set; }

    [MaxLength(TaskConstants.MaxDescriptionLength, ErrorMessage = "too long title (4096 max)")]
    public string? Description { get; set; }

    [Range(1, TaskConstants.MaxLevel, ErrorMessage = "possible task difficulty level values: 1-9")]
    public int SuggestedLevel { get; set; } = 9;

    public string[] Tags { get; set; } = Array.Empty<string>();

    public TaskVisibility Visibility { get; set; } = TaskVisibility.Private;
}

public record EditTaskDto: CreateTaskDto
{
    [Required]
    public Guid TaskId { get; }
}
