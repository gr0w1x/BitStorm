using Microsoft.AspNetCore.Components;
using WebClient.Store.Common;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.SignTemplate;

public abstract class SignTemplateBase<TState, TDto>: FormComponent<TState, TDto>
    where TState: IHasUxState
    where TDto: new ()
{
    [Inject]
    protected ILogger<TState> Logger { get; set; }

    protected override PageAccessType PageAccess => PageAccessType.OnlyPublic;

    protected virtual UxState InitState => UxState.Editable;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Dispatcher.Dispatch(new SetUxState<TState>(
            InitState
        ));
    }

    public override void Dispose()
    {
        Dispatcher.Dispatch(new SetError<TState>(null));
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    protected abstract Task SubmitAction();

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
            await SubmitAction();
            Dispatcher.Dispatch(new SetUxState<TState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Success
            ));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetUxState<TState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Editable
                | UxState.Error
            ));
            Logger.LogError(ex, $"Sign exception: {ex.Message}");
            Dispatcher.Dispatch(new SetError<TState>(ex.Message.Length > 0 ? ex.Message : "Server error"));
        }
    }
}
