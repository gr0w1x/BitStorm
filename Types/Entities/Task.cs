namespace Types.Entities;

public interface ITask: IHasId
{
    string Title { get; set; }
}

public interface ITaskLanguageDetails
{
    Guid TaskId { get; }
    Guid LanguageId { get; }
    string? Details { get; set; }
}

public interface ITaskImplementation
{
    Guid TaskId { get; }
    Guid LanguageId { get; }
    Guid LanguageVersionId { get; }

    string InitialSolution { get; }
    string CompleteSolution { get; }

    string PreloadedCode { get; }

    string ExampleTests { get; }
    string Tests { get; }
}
