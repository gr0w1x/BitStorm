using System.Reflection;
using Types.Dtos;
using Types.Attributes;

namespace Types.Languages;

public static class CodeLanguages
{
    public static readonly LanguageDto[] Languages;
    public static readonly VersionDto[] Versions;

    public static readonly IDictionary<string, LanguageDto> LanguagesDictionary;

    public static readonly IDictionary<Type, LanguageDto> LanguagesByType;
    public static readonly IDictionary<Type, VersionDto> VersionsByType;

    static CodeLanguages()
    {
        VersionsByType = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Select(type => (type, specification: type.GetCustomAttribute(typeof(LanguageSpecificationAttribute))))
            .Where(tuple => tuple.specification != null)
            .ToDictionary(tuple => tuple.type, tuple =>
            {
                LanguageSpecificationAttribute version = (LanguageSpecificationAttribute)tuple.specification!;
                return new VersionDto
                {
                    Language = version.Language,
                    Version = version.Version,
                    ExecutionTimeout = version.ExecutionTimeout,
                    ExecutorQueue = version.ExecutorExchange,
                    RamSizeLimitMb = version.RamSizeLimitMb
                };
            });

        var versionsByLanguage =
            VersionsByType.Values
                .GroupBy(version => version.Language)
                .ToDictionary(versions => versions.Key, versions => versions.ToArray());

        LanguagesByType = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Select(type => (type, language: type.GetCustomAttribute(typeof(LanguageAttribute))))
            .Where(tuple => tuple.language != null)
            .ToDictionary(tuple => tuple.type, tuple =>
            {
                LanguageAttribute language = (LanguageAttribute)tuple.language!;
                var versions = versionsByLanguage[language.Code];
                return new LanguageDto
                {
                    Code = language.Code,
                    Name = language.Name,
                    DefaultVersion = versions.First(version => version.Version == language.DefaultVersion),
                    Versions = versions,
                };
            });

        Languages =
            LanguagesByType.Values.ToArray();

        Versions =
            VersionsByType.Values.ToArray();

        LanguagesDictionary =
            Languages
                .ToDictionary(language => language.Code);
    }

    public static VersionDto? GetVersionDto(string language, string version)
        => Array.Find(Versions, v => v.Language == language && v.Version == version);
}
