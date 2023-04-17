namespace Types.Entities;

public interface ISolution: ICreated
{
    Guid UserId { get; }
    Guid TaskId { get; }
    string Language { get; }
    string Version { get; }

    string SolutionCode { get; }
}
