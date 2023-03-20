using Fluxor;
using Microsoft.AspNetCore.Components;
using Types.Languages;
using WebClient.Store.Pages.ImplementationsPage;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Tasks.ImplementationsPage;

public record ImplementationDto
{
    public string Details { get; set; } = "";
    public string TestCases { get; set; } = "";
    public string ExampleTestCases { get; set; } = "";
    public string CompletedSolution { get; set; } = "";
    public string InitialSolution { get; set; } = "";
    public string Preloaded { get; set; } = "";
}

public partial class ImplementationsPage: ReduxComponent<ImplementationsPageState>, IDisposable
{
    [Parameter]
    public string TaskId { get; set; }

    public string DetailsTab { get; set; } = "details";
    public string SolutionsTab { get; set; } = "completed";

    private IDictionary<SelectedLanguageVersion, ImplementationDto> Implementations =
        new Dictionary<SelectedLanguageVersion, ImplementationDto>();

    public SelectedLanguageVersion LanguageVersion { get; set; }
    public ImplementationDto CurrentImplementation => Implementations![LanguageVersion!];

    public Task OnLanguageVersionChanged(SelectedLanguageVersion languageVersion)
    {
        if (!Implementations!.ContainsKey(languageVersion))
        {
            Implementations[languageVersion] = new ImplementationDto();
        }
        LanguageVersion = languageVersion;
        return Task.CompletedTask;
    }

    protected override void OnInitialized()
    {
        var first = CodeLanguages.Languages.First();
        LanguageVersion = new SelectedLanguageVersion()
        {
            Language = first.Code,
            Version = first.DefaultVersion.Version,
        };
        Implementations[LanguageVersion] = new ImplementationDto();
        base.OnInitialized();
    }
}
