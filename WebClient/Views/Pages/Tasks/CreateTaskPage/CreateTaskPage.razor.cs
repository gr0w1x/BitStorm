using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Types.Dtos;
using Types.Entities;
using WebClient.Services;
using WebClient.Store.Pages.CreateTaskPage;
using WebClient.Views.Components;
using WebClient.Views.Pages.Tasks.TaskFormTemplate;

namespace WebClient.Views.Pages.Tasks.CreateTaskPage;

public record CreateTaskDtoWithTagsSeparated: CreateTaskDto
{
    [JsonIgnore]
    private string _tagsSeparated = string.Empty;

    [JsonIgnore]
    public string TagsSeparated
    {
        get => _tagsSeparated;
        set
        {
            _tagsSeparated = value;
            Tags = value.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
        }
    }

    public CreateTaskDtoWithTagsSeparated()
    {
        Title = null;
        SuggestedLevel = 9;
        Visibility = TaskVisibility.Private;
        Description = null;
        Tags = Array.Empty<string>();
    }
}

public partial class CreateTaskPage: TaskFormTemplateBase<CreateTaskPageState, CreateTaskDtoWithTagsSeparated>
{
    [Inject]
    protected TasksService TasksService { get; set; }

    protected override PageAccessType PageAccess => PageAccessType.OnlyPrivate;

    public override async Task<Guid> Action() => (await TasksService.CreateTask(_dto with
    {
        Description = string.IsNullOrWhiteSpace(_dto.Description) ? null : _dto.Description
    })).Id;
}
