using Microsoft.AspNetCore.Components;
using Types.Dtos;
using WebClient.Constants;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.Pages.CreateTaskPage;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Tasks.CreateTaskPage;

public record CreateTaskDtoWithTagsSeparated: CreateTaskDto
{
    private string _tagsSeparated = string.Empty;
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
}

public partial class CreateTaskPage
{
    [Inject]
    protected TasksService TasksService { get; set; }

    protected override PageAccessType PageAccess => PageAccessType.OnlyPrivate;

    public virtual async Task OnSubmit()
    {
        try
        {
            Dispatcher.Dispatch(new SetUxState<CreateTaskPageState>(
                (ComponentState.Value.UxState
                | UxState.Loading)
                & ~UxState.Error
                & ~UxState.Editable
            ));
            Dispatcher.Dispatch(new SetError<CreateTaskPageState>(null));

            var task = await TasksService.CreateTask(_dto);

            Dispatcher.Dispatch(new SetUxState<CreateTaskPageState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Success
            ));

            Navigation.NavigateTo(Routes.TaskPage(task.Id));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetUxState<CreateTaskPageState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Editable
                | UxState.Error
            ));
            Logger.LogError(ex, ex.Message);
            Dispatcher.Dispatch(new SetError<CreateTaskPageState>(ex.Message.Length > 0 ? ex.Message : "Server error"));
        }
    }
}
