using Types.Attributes;
using Types.Constants;

namespace Types.Languages;

[Language(LanguageCodes.JavaScript, "JavaScript", "Node v8.1.3")]
public record JavaScript;

[LanguageSpecification(LanguageCodes.JavaScript, "Node v8.1.3", "javascript-v18-15-0", 15000, 512)]
public record JavaScript_Node_v18_15_0;
