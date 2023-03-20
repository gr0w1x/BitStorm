
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Types.Dtos;
using WebClient.Services;

namespace WebClient.Views.Pages.Sign.SignUpPage;

public partial class SignUpPage
{
    [Inject]
    protected AuthService AuthService { get; set; }

    protected override async Task SubmitAction()
    {
        await AuthService.SignUp(_dto);
    }
}
