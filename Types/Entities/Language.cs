namespace Types.Entities;

public interface ILanguage : IHasId<string>
{
    String Name { get; }
    Guid DefaultLanguageVersion { get; }
}

public interface ILanguageVersion
{
}
