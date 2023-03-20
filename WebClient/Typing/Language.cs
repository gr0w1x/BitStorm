namespace WebClient.Typing;

public record SelectedLanguageVersion
{
    public string Language { get; init; }
    public string Version { get; init; }
}
