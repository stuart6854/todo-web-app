using Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace WebApp.Components.Pages.Tasks;

public partial class TaskDetail : IDisposable
{
    [Inject]
    private ILogger<TaskDetail> Logger { get; set; }
    [Inject]
    private ApiClient ApiClient { get; set; }
    [Inject]
    private NavigationManager NavManager { get; set; }

    [Parameter]
    public string TaskId { get; set; }
    private ProjectTask Task { get; set; }

    protected override async Task OnInitializedAsync()
    {
        NavManager.LocationChanged += OnLocationChanged;

        Logger.LogInformation("Getting task: {taskId}", TaskId);
        var res = await ApiClient.GetFromJsonAsync<ProjectTask>("/api/tasks/" + TaskId);
        if (res.Success)
        {
            Logger.LogInformation("Successfully got task: {taskId}", TaskId);
            Task = res.Data;
        }
        else
        {
            Logger.LogError("Failed to get task {taskId}: {errorMessage}", TaskId, res.ErrorMessage);
        }
    }

    public void Dispose()
    {
        NavManager.LocationChanged -= OnLocationChanged;
    }

    private async void OnLocationChanged(object sender, LocationChangedEventArgs e)
    {
        var res = await ApiClient.PutAsync<ProjectTask, ProjectTask>("/api/tasks/" + TaskId, Task);
        if (res.Success)
        {
            Logger.LogInformation("Successfully updated task: {taskId}", TaskId);
        }
        else
        {
            Logger.LogError("Failed to update task {taskId}: {errorMessage}", TaskId, res.ErrorMessage);
        }
    }
}