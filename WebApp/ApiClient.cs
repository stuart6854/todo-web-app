using System.Net.Http.Headers;
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

    public async Task<T> GetAsyncFromJson<T>(string url)
    {
        return await httpClient.GetFromJsonAsync<T>(url);
    }

    public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizeHeader();
        var res = await httpClient.PostAsJsonAsync(path, postModel);
        if (res is { IsSuccessStatusCode: true })
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }

        return default;
    }

    public async Task<T1> PutAsync<T1, T2>(string path, T2 putModel)
    {
        await SetAuthorizeHeader();
        var res = await httpClient.PutAsJsonAsync(path, putModel);
        if (res is { IsSuccessStatusCode: true })
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }

        return default;
    }

    public async Task<T> DeleteAsync<T>(string path)
    {
        await SetAuthorizeHeader();
        return await httpClient.DeleteFromJsonAsync<T>(path);
    }
}