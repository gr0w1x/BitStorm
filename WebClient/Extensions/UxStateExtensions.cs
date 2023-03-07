using WebClient.Typing;

namespace WebClient.Extensions;

public static class UxStateExtensions
{
    public static bool Is(this UxState state, UxState another)
        => (state & another) == another;
}
