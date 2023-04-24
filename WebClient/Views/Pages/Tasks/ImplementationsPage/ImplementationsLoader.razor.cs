using Microsoft.AspNetCore.Components;
using Types.Entities;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.Pages.ImplementationsPage;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Tasks.ImplementationsPage;

public partial class ImplementationsLoader: PageComponent<ImplementationsPageState>, IDisposable
{
    [Parameter]
    public string TaskId { get; set; }

    [Inject]
    protected TasksService TasksService { get; set; }
    [Inject]
    protected TaskImplementationsService TaskImplementationsService { get; set; }

    private List<TaskImplementation> Implementations { get; set; }

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        await LoadTask();
    }

    public async Task LoadTask()
    {
        Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Loading));
        try
        {
            if (TaskId == null)
            {
                throw new Exception();
            }
            var taskId = new Guid(TaskId);
            Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Loading));
            var task = await TasksService.Get(taskId);
            Implementations = (await TaskImplementationsService.GetImplementations(taskId)).ToList();
            Dispatcher.Dispatch(new SetTaskAction(task));
            Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Success | (task != null ? UxState.Editable : UxState.None)));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetError<ImplementationsPageState>(ex.Message.Length > 0 ? ex.Message : "server error"));
            Dispatcher.Dispatch(new SetUxState<ImplementationsPageState>(UxState.Error));
        }
    }
}
