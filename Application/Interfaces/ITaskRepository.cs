﻿using Domain;

namespace Application;

public interface ITaskRepository
{
    Task<IReadOnlyList<ProjectTask>> GetAllTasks();
    Task<IReadOnlyList<ProjectTask>> GetAllTasksByProjectId(Guid projectId);
    Task<ProjectTask> GetTask(Guid id);
    Task<ProjectTask> CreateTask(ProjectTaskModel projectTask);
}