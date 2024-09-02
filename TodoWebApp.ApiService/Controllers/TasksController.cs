using Microsoft.AspNetCore.Mvc;
using TodoWebApp.BL.Services;
using TodoWebApp.Models.Models;

namespace TodoWebApp.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BaseResponseModel>> GetTasks()
    {
        var tasks = await taskService.GetTasks();
        return Ok(new BaseResponseModel { Success = true, Data = tasks });
    }
}