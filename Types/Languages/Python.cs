using Types.Attributes;
using Types.Constants;

namespace Types.Languages;

[Language(LanguageCodes.Python, "Python", "3.10")]
public record PythonLanguage;

[LanguageSpecification(LanguageCodes.Python, "3.8", "python-3-8", 15000, 512)]
public record Python_3_8;

[LanguageSpecification(LanguageCodes.Python, "3.10", "python-3-10", 15000, 512)]
public record Python_3_10;
