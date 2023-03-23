using Microsoft.AspNetCore.Components;
using Types.Dtos;
using WebClient.Typing;

namespace WebClient.Views.Components.LanguageSelect;

public partial class LanguageSelect
{
    [Parameter]
    public IDictionary<string, LanguageDto> LanguagesDictionary { get; set; }

    [Parameter]
    public SelectedLanguageVersion SelectedLanguageVersion { get; set; }
    [Parameter]
    public EventCallback<SelectedLanguageVersion> SelectedLanguageVersionChanged { get; set; }

    [Parameter]
    public bool UseVersion { get; set; } = true;

    protected IEnumerable<string> LanguageCodes => LanguagesDictionary.Keys;

    protected IEnumerable<string> Versions => LanguagesDictionary[SelectedLanguageVersion.Language]
        .Versions
        .Select(version => version.Version);

    public async Task OnLanguageChanged(string language)
    {
        if (language != SelectedLanguageVersion.Language)
        {
            SelectedLanguageVersion newSelected = new()
            {
                Language = language,
                Version = LanguagesDictionary[language].DefaultVersion.Version,
            };
            SelectedLanguageVersion = newSelected;
            await SelectedLanguageVersionChanged.InvokeAsync(newSelected);
        }
    }

    public async Task OnVersionChanged(string version)
    {
        if (version != SelectedLanguageVersion.Version)
        {
            SelectedLanguageVersion newSelected = SelectedLanguageVersion with
            {
                Version = version,
            };
            SelectedLanguageVersion = newSelected;
            await SelectedLanguageVersionChanged.InvokeAsync(newSelected);
        }
    }
}
