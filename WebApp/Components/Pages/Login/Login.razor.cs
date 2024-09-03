using Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Authentication;

namespace WebApp.Components.Pages.Login;

public partial class Login
{
    [Inject]
    private ApiClient ApiClient { get; set; }
    [Inject]
    private NavigationManager NavManager { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    private LoginModel LoginModel { get; set; } = new();

    private async Task HandleLogin()
    {
        var res = await ApiClient.PostAsync<LoginResponseModel, LoginModel>("/api/auth/login", LoginModel);
        if (res is { Token: not null })
        {
            await ((CustomAuthStateProvider)AuthStateProvider).MarkUserAsAuthenticated(res.Token);
            NavManager.NavigateTo("/");
        }
    }
}