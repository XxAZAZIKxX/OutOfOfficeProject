using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IEmployeeRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public class DbEmployeeRepository(DataContext dataContext) : IEmployeeRepository
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
}