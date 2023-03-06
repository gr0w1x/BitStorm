using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Types.Entities;

namespace CommonServer.Data.Models;

public record User() : IUser, IUserCredentials
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public UserRoles Roles { get; set; }

    [EmailAddress]
    [Required]
    [MinLength(UserConstants.EmailLengthMin)]
    [MaxLength(UserConstants.EmailLengthMax)]
    public string Email { get; set; }

    [Required]
    [MinLength(UserConstants.UsernameLengthMin)]
    [MaxLength(UserConstants.UsernameLenghtMax)]
    public string Username { get; set; }

    [Required]
    [MinLength(UserConstants.PasswordLengthMin)]
    [MaxLength(UserConstants.PasswordLengthMax)]
    public string Password { get; set; }

    [Required]
    public bool Confirmed { get; set; }

    public string? Name { get; set; }
    public string? Avatar { get; set; }

    [Required]
    public int Trophies { get; set; }

    [Required]
    public DateTimeOffset Registered { get; set; }
    [Required]
    public DateTimeOffset LastSeen { get; set; }
}
