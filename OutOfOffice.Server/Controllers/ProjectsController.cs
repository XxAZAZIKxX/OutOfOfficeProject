using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Repositories;
using OutOfOffice.Server.Services.Implementation.NonInterfaceImpl;

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
        return projectResult.Match(project => project, exceptionHandlingService.HandleException<Project>);
    }

    [HttpPost, Route("add")]
    public async Task<ActionResult<Project>> AddNewProject([FromBody] NewProjectRequest projectRequest)
    {
        var userId = User.Claims.GetUserId();

        var newProject = await projectRepository.AddNewProjectAsync(new Project
        {
            ProjectName = projectRequest.ProjectName,
            ProjectType = projectRequest.ProjectType,
            StartDate = projectRequest.StartDate,
            EndDate = projectRequest.EndDate,
            ProjectManagerId = userId,
            Comment = projectRequest.Comment,
            Status = ProjectStatus.Active
        });
        return newProject;
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

        return result.Match(project => project, exceptionHandlingService.HandleException<Project>);
    }

    [HttpPost, Route("{projectId}/close")]
    public async Task<ActionResult<Project>> CloseProject([FromRoute] ulong projectId)
    {
        var projectResult = await projectRepository.UpdateProjectAsync(projectId, project =>
        {
            project.EndDate = DateTimeOffset.UtcNow;
            project.Status = ProjectStatus.Inactive;
        });

        return projectResult.Match(project => project, exceptionHandlingService.HandleException<Project>);
    }

    [HttpPost, Route("{projectId}/reopen")]
    public async Task<ActionResult<Project>> ReopenProject([FromRoute] ulong projectId)
    {
        var result = await projectRepository.UpdateProjectAsync(projectId, project =>
        {
            project.EndDate = null;
            project.Status = ProjectStatus.Active;
        });

        return result.Match(project => project, exceptionHandlingService.HandleException<Project>);
    }

    [HttpGet, Route("{projectId}/members")]
    public async Task<ActionResult<Employee[]>> GetProjectMembers([FromRoute] ulong projectId)
    {
        var employeesResult = await projectRepository.GetProjectMembersAsync(projectId);
        return employeesResult.Match(employees => employees, exceptionHandlingService.HandleException<Employee[]>);
    }

    [HttpPost, Route("{projectId}/members/add")]
    public async Task<ActionResult<Employee[]>> AddNewProjectMember([FromRoute] ulong projectId,
        [FromQuery] ulong employeeId)
    {
        var result = await projectRepository.AddNewEmployeeToProjectAsync(projectId, employeeId);
        if (result.IsFailed) return await exceptionHandlingService.HandleExceptionAsync(result.Exception);

        var employeesResult = await projectRepository.GetProjectMembersAsync(projectId);
        return employeesResult.Match(employees => employees, exceptionHandlingService.HandleException<Employee[]>);
    }

    [HttpPost, Route("{projectId}/members/remove")]
    public async Task<ActionResult<Employee[]>> RemoveProjectMember([FromRoute] ulong projectId,
        [FromQuery] ulong employeeId)
    {
        var result = await projectRepository.RemoveEmployeeFromProjectAsync(projectId, employeeId);
        if (result.IsFailed) return await exceptionHandlingService.HandleExceptionAsync(result.Exception);

        var employeesResult = await projectRepository.GetProjectMembersAsync(projectId);
        return employeesResult.Match(employees => employees, exceptionHandlingService.HandleException<Employee[]>);
    }
}