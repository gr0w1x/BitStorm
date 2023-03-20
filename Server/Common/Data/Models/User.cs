using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Types.Entities;

namespace CommonServer.Data.Models;

public record User : PublicUser, IUserCredentials
{
    [EmailAddress]
    [Required]
    [MinLength(UserConstants.EmailLengthMin)]
    [MaxLength(UserConstants.EmailLengthMax)]
    [JsonIgnore]
    public string Email { get; set; }

    [Required]
    [MinLength(UserConstants.PasswordLengthMin)]
    [MaxLength(UserConstants.PasswordLengthMax)]
    [JsonIgnore]
    public string Password { get; set; }

    [Required]
    [JsonIgnore]
    public bool Confirmed { get; set; }
}
