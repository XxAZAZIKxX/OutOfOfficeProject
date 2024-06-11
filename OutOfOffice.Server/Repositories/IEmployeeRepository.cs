using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetEmployeeAsync(ulong employeeId);
    Task<Employee[]> GetEmployeesAsync();
}