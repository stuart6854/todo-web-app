using Application;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProjectsController(IProjectService projectService) : ControllerBase
{
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetAllProjectsByUserId([FromRoute] Guid userId)
    {
        var projects = await projectService.GetAllProjectsByUserId(userId);
        return Ok(new ApiResponse<IReadOnlyList<Project>> { Data = projects });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var project = await projectService.GetProject(id);
        return Ok(new ApiResponse<Project> { Data = project });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectModel projectModel)
    {
        var project = await projectService.CreateProject(projectModel);
        return Ok(new ApiResponse<Project> { Data = project });
    }
}