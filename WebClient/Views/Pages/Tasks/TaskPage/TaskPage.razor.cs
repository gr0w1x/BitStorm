using Markdig;
using Microsoft.AspNetCore.Components;
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

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        await LoadTask();
    }

    public async Task LoadTask()
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
            var task = await TasksService.GetTask(taskId);
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
}
