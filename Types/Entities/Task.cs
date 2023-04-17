using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Types.Entities;

public enum TaskVisibility
{
    Private,
    Public
}

public interface ITask: IHasId, ICreated, IUpdated
{
    string Title { get; set; }
    string? Description { get; set; }

    Guid AuthorId { get; }

    int Likes { get; set; }

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

    public int Likes { get; set; }

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

    public bool IsVisible(IUser? user) =>
        Visibility == TaskVisibility.Public ||
        (
            Visibility == TaskVisibility.Private && user != null && (
                (AuthorId == user.Id) ||
                ((user.Roles & UserRoles.Admin) != UserRoles.None)
            )
        );

    public bool CanUpdate(IUser user) =>
        AuthorId == user.Id;

    public bool CanDelete(IUser user) =>
        CanUpdate(user) || ((user.Roles & UserRoles.Admin) != UserRoles.None);

    public bool CanPublish(IUser user) =>
        CanUpdate(user);

    public bool CanApprove(IUser user) =>
        (user.Roles & (UserRoles.Admin | UserRoles.Moderator)) != UserRoles.None &&
        AuthorId != user.Id;
}

public record TaskTag: ITaskTag
{
    [Key]
    public string Id { get; set; }

    [JsonIgnore]
    public List<Task_> Tasks { get; set; } = new List<Task_>();
}
