using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Types.Entities;

[Flags]
public enum UserRoles
{
    None      = 0,
    Common    = 1 << 0,
    Moderator = 1 << 1,
    Admin     = 1 << 2,
}

public interface IUser: IHasId
{
    UserRoles Roles { get; set; }

    string Username { get; set; }
    string? Name { get; set; }

    string? Avatar { get; set; }
    int Trophies { get; }

    public DateTimeOffset Registered { get; }
    public DateTimeOffset LastSeen { get; }
}

public interface IUserCredentials
{
    string Email { get; set; }
    string Password { get; set; }
    bool Confirmed { get; set; }
}

public interface IUserSocial
{
    string? GitHubProfile { get; set; }
    string? TwitterUsername { get; set; }
    string? LinkedIdProfile { get; set; }
    string? StackOverflow { get; set; }
}

public static class UserConstants
{
    public const int UsernameLengthMin = 3;
    public const int UsernameLenghtMax = 50;
    public const int EmailLengthMin = 1;
    public const int EmailLengthMax = 100;
    public const int PasswordLengthMin = 8;
    public const int PasswordLengthMax = 256;
    public static readonly TimeSpan ConfirmPeriod = TimeSpan.FromDays(3);
}

public record PublicUser: IUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public UserRoles Roles { get; set; }

    [Required]
    [MinLength(UserConstants.UsernameLengthMin)]
    [MaxLength(UserConstants.UsernameLenghtMax)]
    public string Username { get; set; }

    public string? Name { get; set; }
    public string? Avatar { get; set; }

    [Required]
    public int Trophies { get; set; }

    [Required]
    public DateTimeOffset Registered { get; set; }
    [Required]
    public DateTimeOffset LastSeen { get; set; }
}

public record UserIdRecord
{
    [Key]
    public Guid Id { get; set; }
}
