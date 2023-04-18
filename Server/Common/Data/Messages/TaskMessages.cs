using System.ComponentModel.DataAnnotations;
using Types.Entities;

namespace CommonServer.Data.Messages;

public record SaveImplementationCodeDtoWithUser: SaveImplementationCodeDto
{
    [Required]
    public Guid User { get; set; }
}
