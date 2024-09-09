using Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using WebApp.Components.Pages.Shared;

namespace WebApp.Components.Pages.Projects;

public partial class ProjectDetail
{
    [Inject]
    private ILogger<Projects> Logger { get; set; }
    [Inject]
    private ApiClient ApiClient { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private NavigationManager NavManager { get; set; }
    [Inject]
    private ISnackbar Snackbar { get; set; }
    [Inject]
    private IDialogService Dialog { get; set; }

    [Parameter]
    public string ProjectId { get; set; }
    private Project SelectedProject { get; set; }

    private IReadOnlyList<ProjectTask> _tasks = [];
    private bool _isLoading;

    private ProjectTaskModel _newProjectTask = new();
    private bool _isCreating;

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Getting project: {projectId}", ProjectId);
        var res = await ApiClient.GetFromJsonAsync<Project>($"/api/projects/{ProjectId}");
        if (res.Success)
        {
            Logger.LogInformation("Received project: {projectId}", res.Data.Id);
            SelectedProject = res.Data;
            StateHasChanged();
            await SyncTaskList();
        }
        else
        {
            Logger.LogError("Failed to get project: {projectId}", ProjectId);
        }
    }

    private async Task SyncTaskList()
    {
        _isLoading = true;

        Logger.LogInformation("Getting tasks for project: {projectId}", ProjectId);
        var res = await ApiClient.GetFromJsonAsync<IReadOnlyList<ProjectTask>>($"/api/tasks/project/{ProjectId}");
        if (res.Success)
        {
            Logger.LogInformation("Got back {count} tasks for project: {projectId}", res.Data.Count, ProjectId);
            _tasks = res.Data;
            StateHasChanged();
        }
        else
        {
            Logger.LogError("Failed to get tasks for project: {projectId}", ProjectId);
        }

        _isLoading = false;
    }

    private async Task HandleCreateTask()
    {
        _isCreating = true;

        _newProjectTask.OwningProjectId = Guid.Parse(ProjectId);

        Logger.LogInformation("Creating new task for project {projectId}", ProjectId);
        var res = await ApiClient.PostAsync<ProjectTask, ProjectTaskModel>("/api/tasks", _newProjectTask);
        if (res.Success)
        {
            Logger.LogInformation("Created new task for project {projectId}: {title}", ProjectId, res.Data.Id);
            _newProjectTask = new ProjectTaskModel();
            await SyncTaskList();
        }
        else
        {
            Logger.LogError("Failed to create task for project {projectId}", ProjectId);
        }

        _isCreating = false;
    }

    private void HandleTaskClick(TableRowClickEventArgs<ProjectTask> clickEventArgs)
    {
        NavManager.NavigateTo(NavManager.BaseUri + "tasks/" + clickEventArgs.Item.Id, forceLoad: true);
    }

    private async Task OnCompleteChanged(ProjectTask task)
    {
        var res = await ApiClient.PutAsync<ProjectTask, ProjectTask>("/api/tasks", task);
        if (res.Success)
        {
            Logger.LogInformation("Successfully updated task: {taskId}", task.Id);
            Snackbar.Add($"Updated task status.", Severity.Success);
        }
        else
        {
            Logger.LogError("Failed to update task {taskId}: {errorMessage}", task.Id, res.ErrorMessage);
            Snackbar.Add($"Failed to update task status.", Severity.Error);
        }
    }

    private async Task OnDeleteTask(ProjectTask task)
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
            await ApiClient.DeleteAsync<bool>("/api/tasks/" + task.Id);
            await SyncTaskList();
        }
    }
}