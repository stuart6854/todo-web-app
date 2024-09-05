using Domain;

namespace Application;

public interface IProjectRepository
{
    Task<Domain.Project> GetProject(Guid id);
    Task<IReadOnlyList<Project>> GetAllProjectsByUserId(Guid userId);
    Task<Project> CreateProject(ProjectModel projectModel);
}