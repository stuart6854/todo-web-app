using System.Net.Http.Headers;
using Domain;
using Domain.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using WebApp.Authentication;
using Task = System.Threading.Tasks.Task;

namespace WebApp;

public class ApiClient(HttpClient httpClient, ProtectedLocalStorage localStorage, AuthenticationStateProvider authStateProvider)
{
    public async Task SetAuthorizeHeader()
    {
        var sessionState = (await localStorage.GetAsync<LoginResponseModel>("sessionState")).Value;
        if (sessionState != null)
        {
            if (sessionState.TokenExpired < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
            }
            else if (sessionState.TokenExpired < DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
            {
                var res = await httpClient.GetFromJsonAsync<ApiResponse<LoginResponseModel>>(
                    $"/api/auth/loginByRefreshToken?refreshToken={sessionState.RefreshToken}"
                );
                if (res.Success)
                {
                    await ((CustomAuthStateProvider)authStateProvider).MarkUserAsAuthenticated(res.Data);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", res.Data.Token);
                }
                else
                {
                    await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                }
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.Token);
            }
        }
    }

    public async Task<ApiResponse<TResponse>> GetFromJsonAsync<TResponse>(string url)
    {
        await SetAuthorizeHeader();
        var res = await httpClient.GetAsync(url);
        if (res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<ApiResponse<TResponse>>(await res.Content.ReadAsStringAsync());
        }

        return new ApiResponse<TResponse> { Success = false, ErrorMessage = res.ReasonPhrase };
    }

    public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TData>(string path, TData data)
    {
        await SetAuthorizeHeader();
        var res = await httpClient.PostAsJsonAsync(path, data);
        if (res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<ApiResponse<TResponse>>(await res.Content.ReadAsStringAsync());
        }

        return new ApiResponse<TResponse> { Success = false, ErrorMessage = res.ReasonPhrase };
    }

    public async Task<ApiResponse<TResponse>> PutAsync<TResponse, TData>(string path, TData data)
    {
        await SetAuthorizeHeader();
        var res = await httpClient.PutAsJsonAsync(path, data);
        return JsonConvert.DeserializeObject<ApiResponse<TResponse>>(await res.Content.ReadAsStringAsync());
    }

    public async Task DeleteAsync<TResponse>(string path)
    {
        await SetAuthorizeHeader();
        await httpClient.DeleteAsync(path);
    }
}