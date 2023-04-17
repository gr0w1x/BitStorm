using Blazorise;
using Markdig;
using Microsoft.AspNetCore.Components;
using Types.Dtos;
using WebClient.Constants;
using WebClient.Extensions;
using WebClient.Models;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.Pages.TaskPage;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Tasks.TaskPage;

public partial class TaskPage
{
    [Parameter]
    public string? TaskId { get; set; }

    protected override PageAccessType PageAccess => PageAccessType.ForAll;

    [Inject]
    public TasksService TasksService { get; set; }

    [Inject]
    public MarkdownPipeline MarkdownPipeline { get; set; }

    [Inject]
    public INotificationService NotificationService { get; set; }

    public bool Editable => !ComponentState.Value.UxState.Is(UxState.Loading);

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        await LoadTask();
    }

    private async Task LoadTask()
    {
        Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Loading));
        try
        {
            if (TaskId == null)
            {
                throw new Exception();
            }
            var taskId = new Guid(TaskId);
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Loading));
            var task = await TasksService.Get(taskId);
            if (task != null)
            {
                ApproveDto = new ApproveTaskDto()
                {
                    Level = task.Level
                };
            }
            Dispatcher.Dispatch(new SetTaskAction(task != null ? new TaskCardModel()
            {
                Task = task
            } : null));
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Success));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetError<TaskPageState>(ex.Message.Length > 0 ? ex.Message : "server error"));
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Error));
        }
    }

    private Modal? deleteModalRef;

    private async Task DeleteTask()
    {
        Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Loading));
        await deleteModalRef?.Hide();
        try
        {
            await TasksService.DeleteTask(new Guid(TaskId!));
            await NotificationService.Success("Deleted!");
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Success));
            Navigation.NavigateTo(Routes.TasksSearchPage);
        }
        catch (Exception e)
        {
            await NotificationService.Error(e.Message, SnackbarConstants.DefaultErrorHeader);
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Error));
        }
    }

    private Modal? publishModalRef;
    private async Task PublishTask()
    {
        Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Loading));
        await publishModalRef?.Hide();
        try
        {
            var id = new Guid(TaskId!);
            await TasksService.PublishTask(new Guid(TaskId!));
            await NotificationService.Success("Published!");
            var task = await TasksService.Get(id);
            Dispatcher.Dispatch(new SetTaskAction(task != null ? ComponentState.Value.TaskModel! with
            {
                Task = task
            } : null));
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Success));
        }
        catch (Exception e)
        {
            await NotificationService.Error(e.Message, SnackbarConstants.DefaultErrorHeader);
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Error));
        }
    }

    private ApproveTaskDto? ApproveDto;
    private ApproveModal? approveModalRef;
    private async Task ApproveTask()
    {
        Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Loading));
        await approveModalRef?.Hide();
        try
        {
            var id = new Guid(TaskId!);
            await TasksService.ApproveTask(id, ApproveDto);
            await NotificationService.Success("Published!");
            var task = await TasksService.Get(id);
            Dispatcher.Dispatch(new SetTaskAction(task != null ? ComponentState.Value.TaskModel! with
            {
                Task = task
            } : null));
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Success));
        }
        catch (Exception e)
        {
            await NotificationService.Error(e.Message, SnackbarConstants.DefaultErrorHeader);
            Dispatcher.Dispatch(new SetUxState<TaskPageState>(UxState.Error));
        }
    }
}
