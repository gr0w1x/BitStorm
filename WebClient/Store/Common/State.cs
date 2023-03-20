using WebClient.Typing;

namespace WebClient.Store.Common;

public interface IHasUxState
{
    UxState UxState { get; }
    UxState InitialState { get; }
}

public interface IHasServerError
{
    public string? ServerError { get; }
}

public abstract record BaseUxServerErrorState<T>: IHasUxState, IHasServerError
{
    public UxState UxState { get; init; }
    public abstract UxState InitialState { get; }
    public string? ServerError { get; init; }

    public BaseUxServerErrorState()
    {
        UxState = InitialState;
    }
}
