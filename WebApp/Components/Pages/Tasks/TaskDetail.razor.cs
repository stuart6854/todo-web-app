using Domain;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebApp.Components.Pages.Shared;

namespace WebApp.Components.Pages.Tasks;

public partial class TaskDetail
{
    [Inject]
    private ILogger<TaskDetail> Logger { get; set; }
    [Inject]
    private ApiClient ApiClient { get; set; }
    [Inject]
    private NavigationManager NavManager { get; set; }
    [Inject]
    private ISnackbar Snackbar { get; set; }
    [Inject]
    private IDialogService Dialog { get; set; }

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
            Snackbar.Add($"Updated task details.", Severity.Success);
        }
        else
        {
            Logger.LogError("Failed to update task {taskId}: {errorMessage}", TaskId, res.ErrorMessage);
            Snackbar.Add($"Failed to update task details.", Severity.Error);
        }
    }

    private async Task OnDeleteTask()
    {
        var parameters = new DialogParameters<DialogTemplate>()
        {
            { d => d.ContentText, "Are you sure you want to delete this task?" },
            { d => d.ButtonText, "Delete" },
            { d => d.Color, Color.Error }
        };
        var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall };
        var dialog = await Dialog.ShowAsync<DialogTemplate>("Delete Task", parameters, options);
        var confirmed = await dialog.GetReturnValueAsync<bool>();
        if (confirmed)
        {
            await ApiClient.DeleteAsync<bool>("/api/tasks/" + Task.Id);
            NavManager.NavigateTo($"/Projects/{Task.ProjectId}");
        }
    }
}