using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface IEmployeeRepository
{
    /// <summary>
    /// Get the employee from the repository
    /// </summary>
    /// <param name="employeeId">The id of the employee</param>
    /// <returns>
    /// The <see cref="Employee"/> if existing;
    /// otherwise <see langword="null"></see>
    /// </returns>
    Task<Employee?> GetEmployeeAsync(ulong employeeId);
    /// <summary>
    /// Get all employees from the repository
    /// </summary>
    /// <returns>Array of <see cref="Employee"/></returns>
    Task<Employee[]> GetEmployeesAsync();
}