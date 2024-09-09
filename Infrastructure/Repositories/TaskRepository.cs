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
            Title = projectTask.Title,
            Description = projectTask.Description,
            ProjectId = projectTask.OwningProjectId,
        };
        dbContext.Tasks.Attach(task);
        await dbContext.SaveChangesAsync();
        return task;
    }

    public async Task<ProjectTask> UpdateTask(ProjectTask projectTask)
    {
        var task = await GetTask(projectTask.Id);

        task.Title = projectTask.Title;
        task.Description = projectTask.Description;
        task.IsComplete = projectTask.IsComplete;

        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync();
        return task;
    }
}