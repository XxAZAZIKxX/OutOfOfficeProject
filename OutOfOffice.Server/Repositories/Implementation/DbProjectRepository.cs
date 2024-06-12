using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IProjectRepository"/> which using <see cref="DataContext"/>
/// </summary>
public class DbProjectRepository(DataContext dataContext) : IProjectRepository
{
    public async Task<Project?> GetProjectAsync(ulong projectId)
    {
        return await dataContext.Projects
            .Include(p=>p.ProjectManager)
            .Include(p=>p.ProjectMembers)
            .ThenInclude(p=>p.Employee)
            .SingleOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project[]> GetProjectsAsync()
    {
        return await dataContext.Projects.AsNoTracking()
            .Include(p=>p.ProjectManager)
            .Include(p=>p.ProjectMembers)
            .ThenInclude(p=>p.Employee)
            .ToArrayAsync();
    }

    public async Task<Project[]> GetProjectsWithEmployeeAsync(ulong employeeId)
    {
        return await dataContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectMembers)
            .ThenInclude(p => p.Employee)
            .Where(p => p.ProjectMembers.Any(m => m.Employee.Id == employeeId))
            .ToArrayAsync();
    }

    public async Task<Project> AddNewProjectAsync(Project project)
    {
        var result = new Project(project);

        await dataContext.AddAsync(result);
        await dataContext.SaveChangesAsync();

        return result;
    }

    public async Task<Project> UpdateProjectAsync(ulong projectId, Action<Project> update)
    {
        var project = await dataContext.Projects.SingleAsync(p => p.Id == projectId);

        update(project);
        await dataContext.SaveChangesAsync();

        return project;
    }

    public async Task<Project> AddNewEmployeeToProjectAsync(ulong projectId, Employee employee)
    {
        var project = await GetProjectAsync(projectId);
        if (project == null) 
            throw new ProjectNotFoundException($"Project with id: {projectId} not found!");

        project.ProjectMembers.Add(new ProjectMember()
        {
            Project = project,
            Employee = employee
        });

        await dataContext.SaveChangesAsync();
        return project;
    }

    public async Task<Project> RemoveEmployeeFromProjectAsync(ulong projectId, ulong employeeId)
    {
        var project = await GetProjectAsync(projectId);
        if (project is null) throw new ProjectNotFoundException($"Project doesnt exists with id: {projectId}");
        var member = project.ProjectMembers.SingleOrDefault(p=>p.EmployeeId == employeeId);
        if (member is null) throw new EmployeeNotFoundException($"Project doesnt contains employee with id: {projectId}");
        project.ProjectMembers.Remove(member);
        await dataContext.SaveChangesAsync();
        return project;
    }
}