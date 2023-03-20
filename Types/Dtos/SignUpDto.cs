using System.ComponentModel.DataAnnotations;
using Types.Entities;

namespace Types.Dtos;

public record SignUpDto()
{
    [EmailAddress(ErrorMessage = "not valid email")]
    [Required(ErrorMessage = "email required")]
    [MinLength(UserConstants.EmailLengthMin, ErrorMessage = "required at least 1 character")]
    [MaxLength(UserConstants.EmailLengthMax, ErrorMessage = "too long email (100 max)")]
    public string Email { get; set; }

    [Required]
    [MinLength(UserConstants.UsernameLengthMin, ErrorMessage = "required at least 3 characters")]
    [MaxLength(UserConstants.UsernameLenghtMax, ErrorMessage = "too long username (50 max)")]
    public string Username { get; set; }

    [Required]
    [MinLength(UserConstants.PasswordLengthMin, ErrorMessage = "required at least 8 characters")]
    [MaxLength(UserConstants.PasswordLengthMax, ErrorMessage = "too long password (256 max)")]
    public string Password { get; set; }
}
