using Domain;

namespace Application;

public class ProjectService(IProjectRepository projectRepository) : IProjectService
{
    public async Task<IReadOnlyList<Project>> GetAllProjectsByUserId(Guid userId)
    {
        return await projectRepository.GetAllProjectsByUserId(userId);
    }

    public async Task<Project> GetProject(Guid id)
    {
        return await projectRepository.GetProject(id);
    }

    public async Task<Project> CreateProject(ProjectModel projectModel)
    {
        return await projectRepository.CreateProject(projectModel);
    }
}