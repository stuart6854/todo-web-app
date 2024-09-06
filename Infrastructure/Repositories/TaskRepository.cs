using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class TaskRepository(AppDbContext dbContext) : ITaskRepository
{
    public async Task<IReadOnlyList<ProjectTask>> GetAllTasks()
    {
        return await dbContext.Tasks.ToListAsync();
    }

    public async Task<IReadOnlyList<ProjectTask>> GetAllTasksByProjectId(Guid projectId)
    {
        return await dbContext.Tasks.Where(t => t.ProjectId == projectId).ToListAsync();
    }

    public async Task<ProjectTask> GetTask(Guid id)
    {
        return await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id) ?? new ProjectTask();
    }

    public async Task<ProjectTask> CreateTask(ProjectTaskModel projectTask)
    {
        var task = new ProjectTask
        {
            Description = projectTask.Description,
            ProjectId = projectTask.OwningProjectId,
        };
        dbContext.Tasks.Attach(task);
        await dbContext.SaveChangesAsync();
        return task;
    }
}