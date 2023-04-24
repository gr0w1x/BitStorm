using System.ComponentModel.DataAnnotations;
using Types.Dtos;
using Types.Entities;

namespace CommonServer.Data.Messages;

public record SaveImplementationCodeMessage: SaveImplementationCodeDto
{
    [Required]
    public SaveImplementationCodeDto Dto { get; set; }

    [Required]
    public UserClaims User { get; set; }
}
