using Domain;

namespace Application;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<IReadOnlyList<ProjectTask>> GetAllTasks()
    {
        return await taskRepository.GetAllTasks();
    }

    public async Task<IReadOnlyList<ProjectTask>> GetAllTasksByProjectId(Guid projectId)
    {
        return await taskRepository.GetAllTasksByProjectId(projectId);
    }

    public async Task<ProjectTask> GetTask(Guid id)
    {
        return await taskRepository.GetTask(id);
    }

    public async Task<ProjectTask> CreateTask(ProjectTaskModel projectTask)
    {
        return await taskRepository.CreateTask(projectTask);
    }

    public async Task<ProjectTask> UpdateTask(ProjectTask projectTask)
    {
        return await taskRepository.UpdateTask(projectTask);
    }

    public async Task DeleteTask(Guid id)
    {
        await taskRepository.DeleteTask(id);
    }
}