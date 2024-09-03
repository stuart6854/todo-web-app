namespace Application;

public interface ITaskRepository
{
    Task<IReadOnlyList<Domain.Task>> GetAllTasks();
    Task<Domain.Task> GetTask(Guid id);
}