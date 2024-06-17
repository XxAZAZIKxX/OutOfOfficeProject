using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;

namespace OutOfOffice.Server.Repositories;

public interface IProjectRepository
{
    /// <summary>
    /// Get project from the repository
    /// </summary>
    /// <param name="projectId">ID of the requested project</param>
    /// <returns>
    /// The <see cref="Project"/> if exists
    /// </returns>
    /// <exception cref="ProjectNotFoundException"></exception>
    Task<Result<Project>> GetProjectAsync(ulong projectId);

    /// <summary>
    /// Get all projects from the repository
    /// </summary>
    /// <returns>Array of projects</returns>
    Task<Project[]> GetProjectsAsync();

    /// <summary>
    /// Get all projects of which the specified employee is a member
    /// </summary>
    /// <param name="employeeId">ID of the employee</param>
    /// <returns>Array of projects</returns>
    /// <exception cref="EmployeeNotFoundException"></exception>
    Task<Result<Project[]>> GetProjectsWithEmployeeAsync(ulong employeeId);

    /// <summary>
    /// Adds new project to the repository
    /// </summary>
    /// <param name="project">New Project to add</param>
    /// <returns>New project</returns>
    Task<Project> AddNewProjectAsync(Project project);

    /// <summary>
    /// Updates the specified project
    /// </summary>
    /// <param name="projectId">ID of the project to update</param>
    /// <param name="update">Action that applies to the project</param>
    /// <returns>Updated project</returns>
    /// <exception cref="ProjectNotFoundException"></exception>
    Task<Result<Project>> UpdateProjectAsync(ulong projectId, Action<Project> update);

    /// <summary>
    /// Adds new employee as a member of existing project
    /// </summary>
    /// <param name="projectId">ID of the project</param>
    /// <param name="employeeId">ID of the employee to add</param>
    /// <returns>
    /// <see langword="true"/> if added;
    /// <see langword="false"/> if employee is already member
    /// </returns>
    /// <exception cref="ProjectNotFoundException"></exception>
    /// <exception cref="EmployeeNotFoundException"></exception>
    Task<Result<bool>> AddNewEmployeeToProjectAsync(ulong projectId, ulong employeeId);

    /// <summary>
    /// Remove the employee from members of existing project
    /// </summary>
    /// <param name="projectId">ID of the project</param>
    /// <param name="employeeId">ID of the employee</param>
    /// <returns>
    /// <see langword="true"/> if removed;
    /// <see langword="false"/> if employee is already not a member
    /// </returns>
    /// <exception cref="ProjectNotFoundException"></exception>
    /// <exception cref="EmployeeNotFoundException"></exception>
    Task<Result<bool>> RemoveEmployeeFromProjectAsync(ulong projectId, ulong employeeId);

    /// <summary>
    /// Get members of project from the repository
    /// </summary>
    /// <param name="projectId">ID of the requested project</param>
    /// <returns>
    /// An array of <see cref="Employee"/> objects if the project exists; 
    /// </returns>
    /// <exception cref="ProjectNotFoundException"></exception>
    Task<Result<Employee[]>> GetProjectMembersAsync(ulong projectId);
}