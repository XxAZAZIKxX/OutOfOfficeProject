using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Data;
using OutOfOffice.Server.Data.Models;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IProjectRepository"/> which using <see cref="DataContext"/>
/// </summary>
public class DbProjectRepository(DataContext dataContext) : IProjectRepository
{
    public async Task<Result<Project>> GetProjectAsync(ulong projectId)
    {
        var singleOrDefault = await dataContext.Projects
            .Include(p => p.ProjectManager)
            .SingleOrDefaultAsync(p => p.Id == projectId);

        if (singleOrDefault == null)
            return new ProjectNotFoundException($"Project with id `{projectId}` not found!");

        return singleOrDefault;
    }

    public async Task<Project[]> GetProjectsAsync()
    {
        return await dataContext.Projects.AsNoTracking()
            .Include(p => p.ProjectManager)
            .ToArrayAsync();
    }

    public async Task<Project[]> GetProjectsWithEmployeeAsync(ulong employeeId)
    {
        return await dataContext.ProjectMembers
            .AsNoTracking()
            .Include(p => p.Project)
            .ThenInclude(p => p.ProjectManager)
            .Where(p => p.EmployeeId == employeeId)
            .Select(p => p.Project)
            .ToArrayAsync();
    }

    public async Task<Project> AddNewProjectAsync(Project project)
    {
        await dataContext.AddAsync(project);
        await dataContext.SaveChangesAsync();

        return project;
    }

    public async Task<Result<Project>> UpdateProjectAsync(ulong projectId, Action<Project> update)
    {
        var project = await GetProjectAsync(projectId);
        if (project.IsFailed) return project;

        update(project.Value);
        await dataContext.SaveChangesAsync();

        return project;
    }

    public async Task<Result<bool>> AddNewEmployeeToProjectAsync(ulong projectId, ulong employeeId)
    {
        var any = await IsEmployeeMemberOfProjectAsync(projectId, employeeId);

        return await any.Match<Task<Result<bool>>>(async isMember =>
        {
            if (isMember) return false;

            await dataContext.ProjectMembers.AddAsync(new ProjectMember()
            {
                EmployeeId = employeeId,
                ProjectId = projectId
            });
            await dataContext.SaveChangesAsync();
            return true;
        }, exception => Task.FromResult<Result<bool>>(exception));
    }

    public async Task<Result<bool>> RemoveEmployeeFromProjectAsync(ulong projectId, ulong employeeId)
    {
        var any = await IsEmployeeMemberOfProjectAsync(projectId, employeeId);

        return await any.Match<Task<Result<bool>>>(async isMember =>
        {
            if (isMember is false) return false;

            var single = await dataContext.ProjectMembers.SingleAsync(p => p.EmployeeId == employeeId &&
                                                                           p.ProjectId == projectId);
            dataContext.ProjectMembers.Remove(single);
            await dataContext.SaveChangesAsync();
            return true;
        }, exception => Task.FromResult<Result<bool>>(exception));
    }

    private async Task<Result<bool>> IsEmployeeMemberOfProjectAsync(ulong projectId, ulong employeeId)
    {
        var isProjectExists = await dataContext.Projects.AnyAsync(p => p.Id == projectId);
        if (isProjectExists is false)
            return new ProjectNotFoundException($"Project with id `{projectId}` not found!");

        var isEmployeeExists = await dataContext.Employees.AnyAsync(p => p.Id == employeeId);
        if (isEmployeeExists is false)
            return new EmployeeNotFoundException($"Employee with id `{projectId}` not found!");

        var anyAsync = await dataContext.ProjectMembers.AnyAsync(p => p.EmployeeId == employeeId &&
                                                                      p.ProjectId == projectId);
        return anyAsync;
    }
}