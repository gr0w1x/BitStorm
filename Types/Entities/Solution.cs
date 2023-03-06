namespace Types.Entities;

public interface ISolution
{
    Guid UserId { get; }
    Guid TaskImplementationId { get; }

    string SolutionCode { get; }
}
