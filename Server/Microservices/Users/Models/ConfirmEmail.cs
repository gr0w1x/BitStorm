using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommonServer.Data.Models;

namespace Users.Models;

public record ConfirmEmail(User User, string Link);

public record ConfirmRecord(string Email, DateTimeOffset Expired)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}
