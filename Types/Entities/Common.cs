namespace Types.Entities;

public interface IHasId<T>
{
    T Id { get; }
}
public interface IHasId : IHasId<Guid> {}

public interface ICreated
{
    DateTimeOffset CreatedAt { get; set; }
}

public interface IUpdated
{
    DateTimeOffset? UpdatedAt { get; set; }
}
