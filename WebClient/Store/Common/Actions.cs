using WebClient.Typing;

namespace WebClient.Store.Common;

public record SetUxState<T> (UxState State);

public record SetError<T> (string? Error);
