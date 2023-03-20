using Types.Dtos;
using Types.Entities;

namespace WebClient.Models;

public record TaskCardModel
{
    public TaskDto TaskDto { get; set; }
    public IUser? Author { get; set; }
}
