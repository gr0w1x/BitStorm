namespace WebClient.Typing;

[Flags]
public enum UxState
{
    None     = 0,
    Idle     = 1 << 0,
    Loading  = 1 << 1,
    Ideal    = 1 << 2,
    Editable = 1 << 3,
    Success  = 1 << 4,
    Error    = 1 << 5
}
