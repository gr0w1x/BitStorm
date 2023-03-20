using Microsoft.AspNetCore.Components;
using WebClient.Store.Common;
using WebClient.Typing;
using WebClient.Views.Components;

namespace WebClient.Views.Pages.Sign.SignTemplate;

public abstract class SignTemplateBase<TState, TDto>: FormComponent<TState, TDto>
    where TState: IHasUxState
    where TDto: new ()
{
    protected override PageAccessType PageAccess => PageAccessType.OnlyPublic;

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
            Dispatcher.Dispatch(new SetError<TState>(null));
            await SubmitAction();
            Dispatcher.Dispatch(new SetUxState<TState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Success
            ));
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(new SetError<TState>(ex.Message.Length > 0 ? ex.Message : "server error"));
            Dispatcher.Dispatch(new SetUxState<TState>(
                (ComponentState.Value.UxState
                & ~UxState.Loading)
                | UxState.Editable
                | UxState.Error
            ));
        }
    }
}
