namespace Types.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class LanguageAttribute: Attribute
{
    public readonly string Code;
    public readonly string Name;
    public readonly string DefaultVersion;

    public LanguageAttribute(string code, string name, string defaultVersion)
    {
        Code = code;
        Name = name;
        DefaultVersion = defaultVersion;
    }
}
