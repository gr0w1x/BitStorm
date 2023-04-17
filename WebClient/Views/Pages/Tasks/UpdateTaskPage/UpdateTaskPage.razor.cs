using Blazorise;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Types.Dtos;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.Pages.UpdateTaskPage;
using WebClient.Typing;
using WebClient.Views.Components;
using WebClient.Views.Pages.Tasks.TaskFormTemplate;

namespace WebClient.Views.Pages.Tasks.UpdateTaskPage;

public record UpdateTaskDtoWithTagsSeparated: UpdateTaskDto
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

    public UpdateTaskDtoWithTagsSeparated()
    {
        Title = "";
        Level = 9;
        Description = "";
        Tags = Array.Empty<string>();
    }
}

public partial class UpdateTaskPage: TaskFormTemplateBase<UpdateTaskPageState, UpdateTaskDtoWithTagsSeparated>
{
    [Inject]
    protected TasksService TasksService { get; set; }

    [Inject]
    public INotificationService NotificationService { get; set; }

    protected override PageAccessType PageAccess => PageAccessType.OnlyPrivate;

    public override async Task<Guid> Action()
    {
        var id = (await TasksService.UpdateTask(new Guid(TaskId), _dto with
        {
            Title = (ComponentState.Value.Task?.Title != _dto.Title) ? _dto.Title : null,
            Level = (ComponentState.Value.Task?.Level != _dto.Level) ? _dto.Level : null,
            Description = (ComponentState.Value.Task?.Description != _dto.Description) ? _dto.Description : null,
            Tags = (string.Join(", ", ComponentState.Value.Task?.Tags?.Select(tag => tag.Id) ?? new List<string>()) != _dto.TagsSeparated) ? _dto.Tags : null
        })).Id;
        await NotificationService.Success("Updated!");
        return id;
    }

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        await LoadTask();
    }

    public async Task LoadTask()
    {
        Dispatcher.Dispatch(new SetUxState<UpdateTaskPageState>(UxState.Loading));
        try
        {
            if (TaskId == null)
            {
                throw new Exception();
            }
            var taskId = new Guid(TaskId);
            Dispatcher.Dispatch(new SetUxState<UpdateTaskPageState>(UxState.Loading));
            var task = await TasksService.Get(taskId);
            if (task != null)
            {
                _dto.Title = task.Title;
                _dto.Level = task.Level;
                _dto.TagsSeparated = string.Join(", ", task.Tags.Select(tag => tag.Id));
                _dto.Description = task.Description;
            }
            Dispatcher.Dispatch(new SetTaskAction(task));
            Dispatcher.Dispatch(new SetUxState<UpdateTaskPageState>(UxState.Success | (task != null ? UxState.Editable : UxState.None)));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetError<UpdateTaskPageState>(ex.Message.Length > 0 ? ex.Message : "server error"));
            Dispatcher.Dispatch(new SetUxState<UpdateTaskPageState>(UxState.Error));
        }
    }
}
