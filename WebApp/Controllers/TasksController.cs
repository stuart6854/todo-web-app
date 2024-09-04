using Application;
using Domain.Models;
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
        return Ok(new ApiResponse<IReadOnlyList<Domain.Task>> { Data = tasks });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetTask(Guid id)
    {
        var task = await taskService.GetTask(id);
        return Ok(new ApiResponse<Domain.Task> { Data = task });
    }
}