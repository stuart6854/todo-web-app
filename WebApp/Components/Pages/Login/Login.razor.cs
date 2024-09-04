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

    private bool AttemptingLogin { get; set; } = false;
    private string ErrorMessage { get; set; } = string.Empty;

    private async Task HandleLogin()
    {
        AttemptingLogin = true;
        ErrorMessage = string.Empty;
        StateHasChanged();

        var res = await ApiClient.PostAsync<LoginResponseModel, LoginModel>("/api/auth/login", LoginModel);
        if (res.Success)
        {
            await ((CustomAuthStateProvider)AuthStateProvider).MarkUserAsAuthenticated(res.Data.Token);
            NavManager.NavigateTo("/");
        }
        else
        {
            ErrorMessage = res.ErrorMessage;
            StateHasChanged();
        }

        AttemptingLogin = false;
    }
}