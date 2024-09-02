namespace TodoWebApp.Web;

public class ApiClient(HttpClient httpClient)
{
    public async Task<T> GetFromJsonAsync<T>(string url)
    {
        return await httpClient.GetFromJsonAsync<T>(url);
    }
}