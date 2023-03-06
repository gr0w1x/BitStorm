using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Types.Dtos;
using WebClient.Services;

namespace WebClient.Views.Pages.SignUpPage;

public partial class SignUpPage
{
    [Inject]
    protected AuthService AuthService { get; set; }

    protected override async Task SubmitAction()
    {
        var res = await AuthService.SignUp(_dto);
        if (!res.IsSuccessStatusCode)
        {
            throw new Exception((await res.Content.ReadFromJsonAsync<ErrorDto>())?.Message);
        }
    }
}
