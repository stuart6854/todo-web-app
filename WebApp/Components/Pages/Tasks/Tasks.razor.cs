using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages.Tasks;

public partial class Tasks
{
    [Inject]
    private ApiClient ApiClient { get; set; }

    private IReadOnlyList<Domain.ProjectTask> _tasks = [];
    private bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        // Simulate asynchronous loading to demonstrate streaming rendering
        await Task.Delay(500);

        var res = await ApiClient.GetFromJsonAsync<IReadOnlyList<Domain.ProjectTask>>("/api/Tasks");
        if (res.Success)
        {
            _tasks = res.Data;
        }

        IsLoading = false;
    }
}