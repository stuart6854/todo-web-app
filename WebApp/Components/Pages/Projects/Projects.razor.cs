using Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using WebApp.Authentication;
using Task = System.Threading.Tasks.Task;

namespace WebApp.Components.Pages.Projects;

public partial class Projects
{
    [Inject]
    private ILogger<Projects> Logger { get; set; }
    [Inject]
    private ApiClient ApiClient { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private NavigationManager NavManager { get; set; }

    private Guid CurrentUserId { get; set; }

    [Parameter]
    public string ProjectId { get; set; } = null;

    private IReadOnlyList<Project> _projects = [];
    private IReadOnlyList<ProjectTask> _tasks = [];
    private bool _isLoading = false;

    private ProjectModel _newProject = new();
    private ProjectTaskModel _newProjectTask = new();
    private bool _isCreating = false;

    private Project SelectedProject { get; set; }

    protected override async Task OnInitializedAsync()
    {
        CurrentUserId = ((CustomAuthStateProvider)AuthStateProvider).UserId;
        if (ProjectId is null)
        {
            await SyncProjectList();
        }
        else
        {
            Logger.LogInformation("Getting project: {projectId}", ProjectId);
            var res = await ApiClient.GetAsyncFromJson<Project>($"/api/projects/{ProjectId}");
            if (res.Success)
            {
                Logger.LogInformation("Received project {projectId} for user: {userId}", res.Data.Id, CurrentUserId);
                SelectedProject = res.Data;
                StateHasChanged();
                await SyncTaskList();
            }
            else
            {
                Logger.LogError("Failed to get project {projectId} for user: {userId}", ProjectId, CurrentUserId);
            }
        }
    }

    private async Task SyncProjectList()
    {
        _isLoading = true;
        CurrentUserId = ((CustomAuthStateProvider)AuthStateProvider).UserId;

        Logger.LogInformation("Getting projects for user: {userId}", CurrentUserId);
        var res = await ApiClient.GetAsyncFromJson<IReadOnlyList<Project>>($"/api/projects/user/{CurrentUserId}");
        if (res.Success)
        {
            Logger.LogInformation("Got back {count} projects for user: {userId}", res.Data.Count, CurrentUserId);
            _projects = res.Data;
            StateHasChanged();
        }
        else
        {
            Logger.LogError("Failed to get projects for user: {userId}", CurrentUserId);
        }

        _isLoading = false;
    }

    private async Task HandleCreateProject()
    {
        _isCreating = true;

        _newProject.OwningUserId = CurrentUserId;

        Logger.LogInformation("Creating new project for user {userId}", CurrentUserId);
        var res = await ApiClient.PostAsync<Project, ProjectModel>("/api/projects", _newProject);
        if (res.Success)
        {
            Logger.LogInformation("Created new project for user {userId}: {title}", CurrentUserId, res.Data.Name);
            _newProject = new ProjectModel();
            await SyncProjectList();
        }
        else
        {
            Logger.LogError("Failed to create project for user {userId}", CurrentUserId);
        }

        _isCreating = false;
    }

    private void HandleProjectClick(TableRowClickEventArgs<Project> clickEventArgs)
    {
        NavManager.NavigateTo(NavManager.Uri + "/" + clickEventArgs.Item.Id, forceLoad: true);
    }

    private async Task SyncTaskList()
    {
        _isLoading = true;

        Logger.LogInformation("Getting tasks for project: {projectId}", ProjectId);
        var res = await ApiClient.GetAsyncFromJson<IReadOnlyList<ProjectTask>>($"/api/tasks/project/{ProjectId}");
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
        NavManager.NavigateTo(NavManager.BaseUri + "/tasks/" + clickEventArgs.Item.Id, forceLoad: true);
    }
}