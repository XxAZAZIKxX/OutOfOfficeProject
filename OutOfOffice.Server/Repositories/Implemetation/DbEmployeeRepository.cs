using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implemetation;

public class DbEmployeeRepository(DataContext dataContext) : IEmployeeRepository
{
    public async Task<Employee?> GetEmployeeAsync(ulong employeeId)
    {
        return await dataContext.Employees.
                Include(p => p.PeoplePartner)
                .SingleOrDefaultAsync(p => p.Id == employeeId);
    }

    public async Task<Employee[]> GetEmployeesAsync()
    {
        return await dataContext.Employees.ToArrayAsync();
    }
}