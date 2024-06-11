using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Repositories;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("[controller]"), Authorize(Policy = Policies.HrAndProjectManagerPolicy)]
public class ProjectsController(
        IProjectRepository projectRepository,
        IEmployeeRepository employeeRepository
        ) : ControllerBase
{
    [HttpGet, Route("get")]
    public async Task<IActionResult> GetAllProjects()
    {
        return Ok(await projectRepository.GetProjectsAsync());
    }

    [HttpPost, Route("add"), Authorize(Policy = Policies.ProjectManagerPolicy)]
    public async Task<IActionResult> AddNewProject([FromBody] NewProjectRequest projectRequest)
    {
        var manager = await employeeRepository.GetEmployeeAsync(User.Claims.GetUserId());
        if (manager is null) throw new Exception("User is not exists");

        var newProject = await projectRepository.AddNewProjectAsync(new Project
        {
            ProjectName = projectRequest.ProjectName,
            ProjectType = projectRequest.ProjectType,
            StartDate = projectRequest.StartDate,
            EndDate = projectRequest.EndDate,
            ProjectManager = manager,
            Comment = projectRequest.Comment,
            Status = projectRequest.Status
        });

        return Ok(newProject);
    }

    [HttpPost, Route("members/add")]
    public async Task<IActionResult> AddNewProjectMember([FromQuery] ulong projectId, [FromQuery] ulong employeeId)
    {
        throw new NotImplementedException();
    }
}