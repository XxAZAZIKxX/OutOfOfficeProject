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
public sealed class ProjectsController(
        IProjectRepository projectRepository,
        ExceptionHandlingService exceptionHandlingService
        ) : ControllerBase
{
    [HttpGet, Route("all"), Authorize(Policy = Policies.HrAndProjectManagerPolicy)]
    public async Task<ActionResult<Project[]>> GetAllProjects()
    {
        return await projectRepository.GetProjectsAsync();
    }

    [HttpGet, Route("{projectId}"), Authorize(Policy = Policies.HrAndProjectManagerPolicy)]
    public async Task<ActionResult<Project>> GetProject([FromRoute] ulong projectId)
    {
        var projectResult = await projectRepository.GetProjectAsync(projectId);
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

    [HttpPost, Route("{projectId}/update")]
    public async Task<ActionResult<Project>> UpdateProject(
        [FromRoute] ulong projectId, [FromBody] UpdateProjectRequest updateProjectRequest)
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
                return NotFound(exception.Message);

            throw exception;
        });
    }

    [HttpPost, Route("{projectId}/close")]
    public async Task<ActionResult<Project>> CloseProject([FromRoute] ulong projectId)
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

    [HttpGet, Route("{projectId}/members")]
    public async Task<ActionResult<Employee[]>> GetProjectMembers([FromRoute] ulong projectId)
    {
        var employeesResult = await projectRepository.GetProjectMembersAsync(projectId);
        return employeesResult.Match<ActionResult<Employee[]>>(employees => employees,
            exception =>
            {
                if (exception is ProjectNotFoundException)
                    return NotFound(exception.Message);

                throw exception;
            });
    }

    [HttpPost, Route("{projectId}/members/add")]
    public async Task<ActionResult<Employee[]>> AddNewProjectMember([FromRoute] ulong projectId, 
        [FromQuery] ulong employeeId)
    {
        var result = await projectRepository.AddNewEmployeeToProjectAsync(projectId, employeeId);
        return await result.Match<Task<ActionResult<Employee[]>>>(async _ =>
        {
            var employeesResult = await projectRepository.GetProjectMembersAsync(projectId);
            if (employeesResult.IsFailed) throw employeesResult.Exception;
            return employeesResult.Value;
        }, exception =>
        {
            if (exception is ProjectNotFoundException or EmployeeNotFoundException)
                return Task.FromResult<ActionResult<Employee[]>>(NotFound(exception.Message));

            throw exception;
        });
    }

    [HttpPost, Route("{projectId}/members/remove")]
    public async Task<ActionResult<Employee[]>> RemoveProjectMember([FromRoute] ulong projectId,
        [FromQuery] ulong employeeId)
    {
        var result = await projectRepository.RemoveEmployeeFromProjectAsync(projectId, employeeId);
        return await result.Match<Task<ActionResult<Employee[]>>>(async _ =>
        {
            var employeesResult = await projectRepository.GetProjectMembersAsync(projectId);
            if (employeesResult.IsFailed) throw employeesResult.Exception;
            return employeesResult.Value;
        }, exception =>
        {
            if (exception is ProjectNotFoundException or EmployeeNotFoundException)
                return Task.FromResult<ActionResult<Employee[]>>(NotFound(exception.Message));

            throw exception;
        });
    }
}