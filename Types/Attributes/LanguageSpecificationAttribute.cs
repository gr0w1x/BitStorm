namespace Types.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class LanguageSpecificationAttribute: Attribute
{
    public readonly string Language;
    public readonly string Version;
    public readonly string ExecutorExchange;
    public readonly TimeSpan ExecutionTimeout;

    public readonly int RamSizeLimitMb;

    public LanguageSpecificationAttribute(
        string language,
        string version,
        string executorExchange,
        int executionTimeout,
        int ramSizeLimitMb
    )
    {
        Language = language;
        Version = version;
        ExecutorExchange = executorExchange;
        ExecutionTimeout = TimeSpan.FromMilliseconds(executionTimeout);
        RamSizeLimitMb = ramSizeLimitMb;
    }
}
