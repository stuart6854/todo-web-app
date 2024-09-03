using Domain.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace WebApp.Components.Pages;

public partial class Tasks
{
    [Inject]
    private ApiClient ApiClient { get; set; }

    private IReadOnlyList<Domain.Task> _tasks = [];

    protected override async Task OnInitializedAsync()
    {
        // Simulate asynchronous loading to demonstrate streaming rendering
        await Task.Delay(500);

        var res = await ApiClient.GetAsyncFromJson<BaseResponseModel>("/api/Tasks");
        if (res is { Success: true })
        {
            _tasks = JsonConvert.DeserializeObject<IReadOnlyList<Domain.Task>>(res.Data.ToString());
        }
    }
}