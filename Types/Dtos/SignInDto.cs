using System.ComponentModel.DataAnnotations;

namespace Types.Dtos;

public record SignInDto()
{
    [Required(ErrorMessage = "email required")]
    public string EmailOrUsername { get; set; }
    [Required(ErrorMessage = "password required")]
    public string Password { get; set; }
}
