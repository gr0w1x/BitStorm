using Types.Attributes;

namespace Types.Languages;

[Language("python", "Python", "3.10")]
public record PythonLanguage;

[LanguageSpecification("python", "3.8", "python-3-8", 15000, 512)]
public record Python_3_8;

[LanguageSpecification("python", "3.10", "python-3-10", 15000, 512)]
public record Python_3_10;
