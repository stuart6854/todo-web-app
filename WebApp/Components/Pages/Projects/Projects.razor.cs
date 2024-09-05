using Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

    private Guid CurrentUserId { get; set; }

    [Parameter]
    public int? ProjectIndex { get; set; } = null;

    private IReadOnlyList<Project> _projects = [];
    private bool _isLoading = false;

    private ProjectModel _newProject = new();
    private bool _isCreating = false;

    protected override async Task OnInitializedAsync()
    {
        CurrentUserId = ((CustomAuthStateProvider)AuthStateProvider).UserId;
        await SyncProjectList();
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
}