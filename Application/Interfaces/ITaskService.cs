namespace Application;

public interface ITaskService
{
    Task<IReadOnlyList<Domain.Task>> GetAllTasks();
    Task<Domain.Task> GetTask(Guid id);
}