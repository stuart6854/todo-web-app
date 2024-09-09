using Domain;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages.Tasks;

public partial class TaskDetail
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
    private bool ChangesMade => Task.Description != InitialDescription || Task.IsComplete != InitialCompleted;
    private bool EnableSaveButton => !ChangesMade;

    private bool InitialCompleted { get; set; }
    private string InitialDescription { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Getting task: {taskId}", TaskId);
        var res = await ApiClient.GetFromJsonAsync<ProjectTask>("/api/tasks/" + TaskId);
        if (res.Success)
        {
            Logger.LogInformation("Successfully got task: {taskId}", TaskId);
            Task = res.Data;
            UpdateInitialValues();
        }
        else
        {
            Logger.LogError("Failed to get task {taskId}: {errorMessage}", TaskId, res.ErrorMessage);
        }
    }

    private void UpdateInitialValues()
    {
        InitialCompleted = Task.IsComplete;
        InitialDescription = Task.Description;
    }

    private async Task UpdateTaskChanges()
    {
        var res = await ApiClient.PutAsync<ProjectTask, ProjectTask>("/api/tasks", Task);
        if (res.Success)
        {
            Logger.LogInformation("Successfully updated task: {taskId}", TaskId);
            UpdateInitialValues();
        }
        else
        {
            Logger.LogError("Failed to update task {taskId}: {errorMessage}", TaskId, res.ErrorMessage);
        }
    }
}