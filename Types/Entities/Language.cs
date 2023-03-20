namespace Types.Entities;

public interface ILanguage : IHasId<string>
{
    String Name { get; }
    string DefaultLanguageVersion { get; }
}

public interface ILanguageVersion: IHasId<string>
{
    String Version { get; }
    string LanguageId { get; }
}
