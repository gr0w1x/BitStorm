using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Types.Dtos;
using WebClient.Services;
using WebClient.Store.User;

namespace WebClient.Views.Pages.SignInPage;

public partial class SignInPage
{
    [Inject]
    protected AuthService AuthService { get; set; }

    protected override async Task SubmitAction()
    {
        var res = await AuthService.SignIn(_dto);
        if (res.IsSuccessStatusCode)
        {
            var tokens = (await res.Content.ReadFromJsonAsync<AccessRefreshTokensDto>())!;
            Dispatcher.Dispatch(new SetTokensAction(tokens));
            return;
        }
        throw new Exception((await res.Content.ReadFromJsonAsync<ErrorDto>())?.Message);
    }
}
