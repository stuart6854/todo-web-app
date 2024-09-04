using Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
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

    private bool IsBusy { get; set; } = false;
    private string ErrorMessage { get; set; } = string.Empty;

    private async Task HandleRegister()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        StateHasChanged();

        var res = await ApiClient.PostAsync<LoginResponseModel, RegisterModel>("/api/auth/register", RegisterModel);
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

        IsBusy = false;
    }
}