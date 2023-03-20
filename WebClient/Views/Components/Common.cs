using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebClient.Services;
using WebClient.Store.Common;
using WebClient.Store.User;
using WebClient.Typing;

namespace WebClient.Views.Components;

public class ReduxComponent<TState>: ComponentBase, IDisposable
{
    [Inject]
    protected IDispatcher Dispatcher { get; set; }

    [Inject]
    protected IState<TState> ComponentState { get; set; }

    protected override void OnInitialized()
    {
        ComponentState.StateChanged += OnStateChanged;
    }

    public virtual void Dispose()
    {
        ComponentState.StateChanged -= OnStateChanged;
        GC.SuppressFinalize(this);
    }

    protected virtual void OnStateChanged(object? sender, EventArgs args)
        => StateHasChanged();
}

public class UxStateComponent<TState>: ReduxComponent<TState>
    where TState: IHasUxState
{
    [Inject]
    protected ILogger<UxStateComponent<TState>> Logger { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ComponentState.Value.UxState != ComponentState.Value.InitialState)
        {
            Dispatcher.Dispatch(new SetUxState<TState>(ComponentState.Value.InitialState));
        }
    }

    public virtual void Dispose()
    {
        base.Dispose();
        if (ComponentState.Value.UxState != ComponentState.Value.InitialState)
        {
            Dispatcher.Dispatch(new SetUxState<TState>(ComponentState.Value.InitialState));
        }
    }
}

[Flags]
public enum PageAccessType
{
    Inaccessible = 0,
    OnlyPublic       = 1 << 0,
    OnlyPrivate      = 1 << 1
}

public class PageComponent<TState>: UxStateComponent<TState>
    where TState: IHasUxState
{
    [Inject]
    protected IState<UserState> UserState { get; set; }

    [Inject]
    protected Navigation Navigation { get; set; }

    protected virtual PageAccessType PageAccess => PageAccessType.OnlyPrivate;

    protected virtual string Redirect => "/";

    protected override void OnInitialized()
    {
        UserState.StateChanged += OnStateChanged;
        base.OnInitialized();
    }

    public override void Dispose()
    {
        UserState.StateChanged -= OnStateChanged;
        base.Dispose();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if ((UserState.Value.Authorized && (PageAccess & PageAccessType.OnlyPrivate) == PageAccessType.Inaccessible)
            || (!UserState.Value.Authorized && (PageAccess & PageAccessType.OnlyPublic) == PageAccessType.Inaccessible))
        {
            Navigation.NavigateTo(Redirect);
        }
    }

    protected void NavigateRedirect()
    {
        Navigation.NavigateTo(Redirect);
    }

    protected void NavigateBackOrRedirect()
    {
        if (Navigation.CanNavigateBack)
        {
            Navigation.NavigateBack();
        }
        else
        {
            NavigateRedirect();
        }
    }
}

public class FormComponent<TState, TDto>: PageComponent<TState>
    where TState: IHasUxState
    where TDto: new ()
{
    protected bool CanEdit => (ComponentState.Value.UxState & UxState.Editable) != UxState.None;

    protected bool _canSubmit;
    protected bool CanSubmit => CanEdit && _canSubmit;

    protected readonly TDto _dto = new();
    public readonly EditContext _context;

    public FormComponent()
    {
        _context = new EditContext(_dto);
    }

    protected override void OnInitialized()
    {
        _context.OnFieldChanged += OnFieldChanged;
        base.OnInitialized();
    }

    public override void Dispose()
    {
        _context.OnFieldChanged -= OnFieldChanged;
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void OnFieldChanged(object? sender, EventArgs args)
    {
        _canSubmit = _context.Validate();
        OnStateChanged(sender, args);
    }
}
