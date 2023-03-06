using System.ComponentModel.DataAnnotations;
using Types.Entities;

namespace Types.Dtos;

public record SignUpDto()
{
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
}
