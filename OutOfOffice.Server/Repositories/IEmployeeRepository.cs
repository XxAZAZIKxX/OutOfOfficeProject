using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Services;

public interface IEmployeeRepository
{
    Task<Employee?> GetEmployeeAsync(ulong employeeId);
    Task<Employee[]> GetEmployeesAsync();
}