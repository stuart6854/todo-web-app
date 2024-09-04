using System.Net.Http.Headers;
using Domain.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;

namespace WebApp;

public class ApiClient(HttpClient httpClient, ProtectedLocalStorage localStorage)
{
    public async Task SetAuthorizeHeader()
    {
        var token = (await localStorage.GetAsync<string>("authToken")).Value;
        if (token != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<ApiResponse<TResponse>> GetAsyncFromJson<TResponse>(string url)
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