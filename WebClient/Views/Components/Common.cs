using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebClient.Services;
using WebClient.Store.User;
using WebClient.Typing;

namespace WebClient.Views.Components;

public interface IHasUxState
{
    UxState UxState { get; }
}

public class Component<TState>: ComponentBase, IDisposable
    where TState: IHasUxState
{
    [Inject]
    protected IDispatcher Dispatcher { get; set; }

    [Inject]
    protected IState<TState> ComponentState { get; set; }

    [Inject]
    protected ILogger<Component<TState>> Logger { get; set; }

    [Inject]
    protected Navigation Navigation { get; set; }

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

[Flags]
public enum PageAccessType
{
    Inaccessible = 0,
    OnlyPublic       = 1 << 0,
    OnlyPrivate      = 1 << 1
}

public class PageComponent<TState>: Component<TState>
    where TState: IHasUxState
{
    [Inject]
    protected IState<UserState> UserState { get; set; }

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
        GC.SuppressFinalize(this);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if ((UserState.Value.Authorized && (PageAccess & PageAccessType.OnlyPrivate) == PageAccessType.Inaccessible)
            || (!UserState.Value.Authorized && (PageAccess & PageAccessType.OnlyPublic) == PageAccessType.Inaccessible))
        {
            if (Navigation.CanNavigateBack)
            {
                Navigation.NavigateBack();
            }
            else
            {
                Navigation.NavigateTo(Redirect);
            }
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
