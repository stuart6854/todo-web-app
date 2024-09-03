using Application;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class TaskRepository(AppDbContext dbContext) : ITaskRepository
{
    public async Task<IReadOnlyList<Domain.Task>> GetAllTasks()
    {
        return await dbContext.Tasks.ToListAsync();
    }

    public async Task<Domain.Task> GetTask(Guid id)
    {
        return await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id) ?? new Domain.Task();
    }
}