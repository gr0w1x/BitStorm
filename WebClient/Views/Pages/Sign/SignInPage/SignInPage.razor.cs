using Microsoft.AspNetCore.Components;
using WebClient.Services;
using WebClient.Store.User;

namespace WebClient.Views.Pages.Sign.SignInPage;

public partial class SignInPage
{
    [Inject]
    protected AuthService AuthService { get; set; }

    protected override async Task SubmitAction()
    {
        Dispatcher.Dispatch(new SetTokensAction(await AuthService.SignIn(_dto)));
    }
}
