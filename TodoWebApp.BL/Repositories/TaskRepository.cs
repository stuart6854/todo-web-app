using Microsoft.EntityFrameworkCore;
using TodoWebApp.Database.Data;
using TodoWebApp.Models.Entities;

namespace TodoWebApp.BL.Repositories;

public interface ITaskRepository
{
    Task<List<TaskModel>> GetTasks();
}

public class TaskRepository(AppDbContext dbContext) : ITaskRepository
{
    public Task<List<TaskModel>> GetTasks()
    {
        return dbContext.Tasks.ToListAsync();
    }
}