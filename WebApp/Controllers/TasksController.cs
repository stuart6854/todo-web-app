using Application;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await taskService.GetAllTasks();
        return Ok(new ApiResponse<IReadOnlyList<ProjectTask>> { Data = tasks });
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetAllTaskByProjectId([FromRoute] Guid projectId)
    {
        var tasks = await taskService.GetAllTasksByProjectId(projectId);
        return Ok(new ApiResponse<IReadOnlyList<ProjectTask>> { Data = tasks });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var task = await taskService.GetTask(id);
        return Ok(new ApiResponse<ProjectTask> { Data = task });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(ProjectTaskModel projectTaskModel)
    {
        var task = await taskService.CreateTask(projectTaskModel);
        return Ok(new ApiResponse<ProjectTask> { Data = task });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTask(ProjectTask projectTaskModel)
    {
        var task = await taskService.UpdateTask(projectTaskModel);
        return Ok(new ApiResponse<ProjectTask> { Data = task });
    }
}