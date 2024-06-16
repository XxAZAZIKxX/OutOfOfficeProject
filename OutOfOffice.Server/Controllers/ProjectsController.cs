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
    public async Task<ActionResult<Project>> GetProject([FromRoute] ulong id)
    {
        var projectResult = await projectRepository.GetProjectAsync(id);
        return projectResult.Match<ActionResult<Project>>(project => project,
            exception =>
            {
                if (exception is ProjectNotFoundException)
                    return NotFound(exception.Message);

                throw exception;
            });
    }

    [HttpPost, Route("add")]
    public async Task<ActionResult<Project>> AddNewProject([FromBody] NewProjectRequest projectRequest)
    {
        var managerResult = await employeeRepository.GetEmployeeAsync(User.Claims.GetUserId());

        return await managerResult.Match<Task<ActionResult<Project>>>(async manager =>
        {
            var newProject = await projectRepository.AddNewProjectAsync(new Project
            {
                ProjectName = projectRequest.ProjectName,
                ProjectType = projectRequest.ProjectType,
                StartDate = projectRequest.StartDate,
                EndDate = projectRequest.EndDate,
                ProjectManager = manager,
                Comment = projectRequest.Comment,
                Status = ProjectStatus.Active
            });
            return newProject;
        }, exception => throw new InvalidOperationException("Logged user is not exists", exception));
    }

    [HttpPost, Route("update")]
    public async Task<ActionResult<Project>> UpdateProject(
        [FromQuery] ulong projectId, [FromBody] UpdateProjectRequest updateProjectRequest)
    {
        var result = await projectRepository.UpdateProjectAsync(projectId, project =>
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

        return result.Match<ActionResult<Project>>(project => project, exception =>
        {
            if (exception is ProjectNotFoundException)
            {
                return NotFound(exception.Message);
            }

            throw exception;
        });
    }

    [HttpPost, Route("close")]
    public async Task<ActionResult<Project>> CloseProject([FromQuery] ulong projectId)
    {
        var projectResult = await projectRepository.UpdateProjectAsync(projectId, project =>
        {
            project.EndDate = DateTimeOffset.UtcNow;
            project.Status = ProjectStatus.Inactive;
        });

        return projectResult.Match<ActionResult<Project>>(project => project, exception =>
        {
            if (exception is ProjectNotFoundException)
                return NotFound(exception.Message);

            throw exception;
        });
    }

    [HttpPost, Route("members/add")]
    public async Task<IActionResult> AddNewProjectMember([FromQuery] ulong projectId, [FromQuery] ulong employeeId)
    {
        var result = await projectRepository.AddNewEmployeeToProjectAsync(projectId, employeeId);
        return result.Match<IActionResult>(b => Ok(b ? "User was added!" : "User was already added!"), exception =>
        {
            if (exception is ProjectNotFoundException or EmployeeNotFoundException)
            {
                return NotFound(exception.Message);
            }

            throw exception;
        });
    }

    [HttpPost, Route("members/remove")]
    public async Task<IActionResult> RemoveProjectMember([FromQuery] ulong projectId, [FromQuery] ulong employeeId)
    {
        var result = await projectRepository.RemoveEmployeeFromProjectAsync(projectId, employeeId);
        return result.Match<IActionResult>(b => Ok(b ? "Member was removed!" : "Member was already removed!"),
            exception =>
            {
                if (exception is ProjectNotFoundException or EmployeeNotFoundException)
                    return NotFound(exception.Message);

                throw exception;
            });
    }
}