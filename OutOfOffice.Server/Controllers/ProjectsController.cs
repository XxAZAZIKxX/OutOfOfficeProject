using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Repositories;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("[controller]"), Authorize(Policy = Policies.ProjectManagerPolicy)]
public class ProjectsController(
        IProjectRepository projectRepository,
        IEmployeeRepository employeeRepository
        ) : ControllerBase
{
    [HttpGet, Route("get"), Authorize(Policy = Policies.HrAndProjectManagerPolicy)]
    public async Task<IActionResult> GetAllProjects()
    {
        return Ok(await projectRepository.GetProjectsAsync());
    }

    [HttpGet, Route("get/{id}"), Authorize(Policy = Policies.HrAndProjectManagerPolicy)]
    public async Task<IActionResult> GetProject([FromRoute] ulong id)
    {
        var project = await projectRepository.GetProjectAsync(id);
        if (project is null) return NotFound($"Project with id {id} not found!");
        return Ok(project);
    }

    [HttpPost, Route("add")]
    public async Task<IActionResult> AddNewProject([FromBody] NewProjectRequest projectRequest)
    {
        var manager = await employeeRepository.GetEmployeeAsync(User.Claims.GetUserId());
        if (manager is null) return NotFound("User is not exists");

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

    [HttpPost, Route("update")]
    public async Task<IActionResult> UpdateProject(
        [FromQuery] ulong projectId, [FromBody] UpdateProjectRequest updateProjectRequest)
    {
        try
        {
            var project = await projectRepository.UpdateProjectAsync(projectId, project =>
            {
                if (updateProjectRequest.ProjectName.HasValue)
                    project.ProjectName = updateProjectRequest.ProjectName.Value;
                if (updateProjectRequest.ProjectType.HasValue)
                    project.ProjectType = updateProjectRequest.ProjectType.Value;
                if (updateProjectRequest.StartDate.HasValue)
                    project.StartDate = updateProjectRequest.StartDate.Value;
                if (updateProjectRequest.Comment.HasValue)
                    project.Comment = updateProjectRequest.Comment.Value;
            });
            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound($"Project with {projectId} id not found!");
        }
    }

    [HttpPost, Route("close")]
    public async Task<IActionResult> CloseProject([FromQuery] ulong projectId)
    {
        try
        {
            var project = await projectRepository.UpdateProjectAsync(projectId, project =>
            {
                project.EndDate = DateTimeOffset.UtcNow;
                project.Status = ProjectStatus.Inactive;
            });
            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound($"Project with id {projectId} not found!");
        }
    }

    [HttpPost, Route("members/add")]
    public async Task<IActionResult> AddNewProjectMember([FromQuery] ulong projectId, [FromQuery] ulong employeeId)
    {
        var employee = await employeeRepository.GetEmployeeAsync(employeeId);
        if (employee is null) return NotFound($"Employee doesn't exists. Employee id: {employeeId}");
        try
        {
            await projectRepository.AddNewEmployeeToProjectAsync(projectId, employee);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound($"Project with id: {projectId} not found!");
        }
        return Ok(await projectRepository.GetProjectAsync(projectId));
    }

    [HttpPost, Route("members/remove")]
    public async Task<IActionResult> RemoveProjectMember([FromQuery] ulong projectId, [FromQuery] ulong employeeId)
    {
        try
        {
            var project = await projectRepository.RemoveEmployeeFromProjectAsync(projectId, employeeId);
            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound($"Project with id {projectId} not found!");
        }
        catch (EmployeeNotFoundException)
        {
            return NotFound($"Employee with id {employeeId} not found!");
        }
    }
}