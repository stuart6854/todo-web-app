using TodoWebApp.BL.Repositories;
using TodoWebApp.Models.Entities;

namespace TodoWebApp.BL.Services;

/*
 * Service = Business logic
 */

public interface ITaskService
{
    Task<List<TaskModel>> GetTasks();
}

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public Task<List<TaskModel>> GetTasks()
    {
        return taskRepository.GetTasks();
    }
}