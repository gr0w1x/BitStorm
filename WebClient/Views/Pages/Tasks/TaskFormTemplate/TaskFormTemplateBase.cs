using WebClient.Constants;
using WebClient.Store.Common;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Tasks.TaskFormTemplate;

public abstract partial class TaskFormTemplateBase<TState, TDto>: FormComponent<TState, TDto>
    where TState: IHasUxState, IHasServerError
    where TDto: new ()
{
    public abstract Task<Guid> Action();

    protected override PageAccessType PageAccess => PageAccessType.OnlyPrivate;

    public async Task OnSubmit()
    {
        try
        {
            Dispatcher.Dispatch(new SetUxState<TState>(
                (ComponentState.Value.UxState
                | UxState.Loading)
                & ~UxState.Error
                & ~UxState.Editable
            ));
            Dispatcher.Dispatch(new SetError<TState>(null));

            var taskId = await Action();

            Dispatcher.Dispatch(new SetUxState<TState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Success
            ));

            Navigation.NavigateTo(Routes.TaskPage(taskId));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetUxState<TState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Editable
                | UxState.Error
            ));
            Logger.LogError(ex, ex.Message);
            Dispatcher.Dispatch(new SetError<TState>(ex.Message.Length > 0 ? ex.Message : "Server error"));
        }
    }
}
