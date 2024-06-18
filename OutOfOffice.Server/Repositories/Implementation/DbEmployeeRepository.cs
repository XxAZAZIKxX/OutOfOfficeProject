using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IEmployeeRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public sealed class DbEmployeeRepository(DataContext dataContext) : IEmployeeRepository
{
    public async Task<Result<Employee>> GetEmployeeAsync(ulong employeeId)
    {
        var singleOrDefault = await dataContext.Employees
            .Include(p => p.PeoplePartner)
            .SingleOrDefaultAsync(p => p.Id == employeeId);

        if (singleOrDefault == null) 
            return new EmployeeNotFoundException($"Employee with id `{employeeId}` not found!");

        return singleOrDefault;
    }

    public async Task<Employee[]> GetEmployeesAsync()
    {
        return await dataContext.Employees.AsNoTracking().ToArrayAsync();
    }

    public async Task<Result<Employee>> UpdateEmployeeAsync(ulong employeeId, Action<Employee> update)
    {
        var employeeResult = await GetEmployeeAsync(employeeId);
        if (employeeResult.IsFailed) return employeeResult.Exception;

        var employee = employeeResult.Value;
        update(employee);

        await dataContext.SaveChangesAsync();

        return employee;
    }
}