using Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Authentication;

namespace WebApp.Components.Pages.Login;

public partial class Register
{
    [Inject]
    private ApiClient ApiClient { get; set; }
    [Inject]
    private NavigationManager NavManager { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }

    private RegisterModel RegisterModel { get; set; } = new();

    private async Task HandleRegister()
    {
        var res = await ApiClient.PostAsync<LoginResponseModel, RegisterModel>("/api/auth/register", RegisterModel);
        if (res is { Token: not null })
        {
            await ((CustomAuthStateProvider)AuthStateProvider).MarkUserAsAuthenticated(res.Token);
            NavManager.NavigateTo("/");
        }
    }
}