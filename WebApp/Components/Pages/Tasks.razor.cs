using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages;

public partial class Tasks
{
    [Inject]
    private ApiClient ApiClient { get; set; }

    private IReadOnlyList<Domain.Task> _tasks = [];
    private bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        // Simulate asynchronous loading to demonstrate streaming rendering
        await Task.Delay(500);

        var res = await ApiClient.GetAsyncFromJson<IReadOnlyList<Domain.Task>>("/api/Tasks");
        if (res.Success)
        {
            _tasks = res.Data;
        }

        IsLoading = false;
    }
}