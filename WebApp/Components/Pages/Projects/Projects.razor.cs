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

    private List<Project> _projects = [];
    private bool _isLoading = false;

    private ProjectModel _newProject = new();
    private bool _isCreating = false;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        CurrentUserId = ((CustomAuthStateProvider)AuthStateProvider).UserId;

        Logger.LogInformation("Getting projects for user: {userId}", CurrentUserId);
        var res = await ApiClient.GetAsyncFromJson<IReadOnlyList<Project>>($"/api/projects/user/{CurrentUserId}");
        if (res.Success)
        {
            Logger.LogInformation("Got back {count} projects for user: {userId}", res.Data.Count, CurrentUserId);
            _projects = res.Data as List<Project> ?? [];
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

        Logger.LogInformation("Creating new project");
        var res = await ApiClient.PostAsync<Project, ProjectModel>("/api/projects", _newProject);
        if (res.Success)
        {
            Logger.LogInformation("Created new project: {title}", res.Data.Name);
            _projects.Add(res.Data);
            _newProject = new ProjectModel();
        }
        else
        {
            Logger.LogError("Failed to create project: {title}", _newProject.Name);
        }

        _isCreating = false;
        StateHasChanged();
    }
}