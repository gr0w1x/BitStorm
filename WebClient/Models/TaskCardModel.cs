using Types.Dtos;
using Types.Entities;

namespace WebClient.Models;

public record TaskCardModel
{
    public Task_ Task { get; set; }
    public IUser? Author { get; set; }
}
