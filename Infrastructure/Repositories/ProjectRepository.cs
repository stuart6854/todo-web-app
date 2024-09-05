using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ProjectRepository(AppDbContext dbContext) : IProjectRepository
{
    public async Task<IReadOnlyList<Project>> GetAllProjectsByUserId(Guid userId)
    {
        return await dbContext.Projects.Include(p => p.OwningUser).Where(p => p.OwningUser.Id == userId).ToListAsync();
    }

    public async Task<Project> GetProject(Guid id)
    {
        return await dbContext.Projects.FirstOrDefaultAsync(t => t.Id == id) ?? new Domain.Project();
    }

    public async Task<Project> CreateProject(ProjectModel projectModel)
    {
        var project = new Project
        {
            Name = projectModel.Name,
            Description = projectModel.Description,
            OwningUserId = projectModel.OwningUserId,
        };
        dbContext.Projects.Attach(project);
        await dbContext.SaveChangesAsync();
        return project;
    }
}