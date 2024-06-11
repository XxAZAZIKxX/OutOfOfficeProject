using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implemetation;

/// <summary>
/// Implementation of <see cref="IProjectRepository"/> which using <see cref="DataContext"/>
/// </summary>
public class DbProjectRepository(DataContext dataContext) : IProjectRepository
{
    public async Task<Project?> GetProjectAsync(ulong projectId)
    {
        return await dataContext.Projects
            .Include(p=>p.ProjectMembers)
            .SingleOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project[]> GetProjectsAsync()
    {
        return await dataContext.Projects.AsNoTracking().ToArrayAsync();
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

    public async Task AddNewEmployeeToProjectAsync(ulong projectId, Employee employee)
    {
        var project = await dataContext.Projects
            .Include(p => p.ProjectMembers)
            .SingleAsync(p => p.Id == projectId);

        project.ProjectMembers.Add(new ProjectMember()
        {
            Project = project,
            Employee = employee
        });

        await dataContext.SaveChangesAsync();
    }
}