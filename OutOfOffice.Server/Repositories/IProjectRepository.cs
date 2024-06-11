using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetProjectAsync(ulong projectId);
    Task<Project[]> GetProjectsAsync();
    Task<Project[]> GetProjectsWithEmployeeAsync(ulong employeeId);
    Task<Project> AddNewProjectAsync(Project project);
    Task<Project> UpdateProjectAsync(ulong projectId, Action<Project> update);
    Task AddNewEmployeeToProjectAsync(ulong projectId, Employee employee);
}