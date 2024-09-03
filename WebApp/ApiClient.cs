namespace WebApp;

public class ApiClient(HttpClient httpClient)
{
    public async Task<T> GetAsyncFromJson<T>(string url)
    {
        return await httpClient.GetFromJsonAsync<T>(url);
    }
}