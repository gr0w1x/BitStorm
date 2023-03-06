namespace Types.Entities;

public interface IHasId<T>
{
    T Id { get; }
}
public interface IHasId : IHasId<Guid> {}

public interface ICreated
{
    DateTime Created { get; }
}
public interface IUpdated
{
    DateTime Updated { get; }
}
