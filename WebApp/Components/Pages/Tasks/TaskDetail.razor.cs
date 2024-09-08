using Domain;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages.Tasks;

public partial class TaskDetail
{
    [Inject]
    private ILogger<TaskDetail> Logger { get; set; }
    [Inject]
    private ApiClient ApiClient { get; set; }

    [Parameter]
    public string TaskId { get; set; }
    private ProjectTask Task { get; set; }

    protected override async Task OnInitializedAsync()
    {
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
}