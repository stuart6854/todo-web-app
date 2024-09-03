namespace Application;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<IReadOnlyList<Domain.Task>> GetAllTasks()
    {
        return await taskRepository.GetAllTasks();
    }

    public async Task<Domain.Task> GetTask(Guid id)
    {
        return await taskRepository.GetTask(id);
    }
}