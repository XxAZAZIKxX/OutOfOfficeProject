using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Server.Core;

public static class Roles
{
    public const string EmployeeRole = nameof(EmployeePosition.Employee);
    public const string HrManagerRole = nameof(EmployeePosition.HrManager);
    public const string ProjectManagerRole = nameof(EmployeePosition.ProjectManager);
    public const string AdministratorRole = nameof(EmployeePosition.Administrator);
}