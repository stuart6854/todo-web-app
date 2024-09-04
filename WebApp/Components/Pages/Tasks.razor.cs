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

        var res = await ApiClient.GetAsyncFromJson<IReadOnlyList<Domain.Task>>("/api/Tasks");
        if (res.Success)
        {
            _tasks = res.Data;
        }
    }
}