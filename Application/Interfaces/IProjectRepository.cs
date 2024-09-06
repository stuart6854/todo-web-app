using Domain;

namespace Application;

public interface IProjectRepository
{
    Task<Project> GetProject(Guid id);
    Task<IReadOnlyList<Project>> GetAllProjectsByUserId(Guid userId);
    Task<Project> CreateProject(ProjectModel projectModel);
}