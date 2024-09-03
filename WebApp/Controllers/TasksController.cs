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
    public async Task<ActionResult<IReadOnlyList<Domain.Task>>> GetAllTasks()
    {
        var tasks = await taskService.GetAllTasks();
        return Ok(BaseResponseModel.Ok(tasks));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IReadOnlyList<Domain.Task>>> GetTask(Guid id)
    {
        var task = await taskService.GetTask(id);
        return Ok(BaseResponseModel.Ok(task));
    }
}