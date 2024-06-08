using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Server.Core;

public static class Policies
{
    public const string EmployeePolicy = nameof(EmployeePosition.Employee);
    public const string HrManagerPolicy = nameof(EmployeePosition.HrManager);
    public const string ProjectManagerPolicy = nameof(EmployeePosition.ProjectManager);
    public const string AdministratorPolicy = nameof(EmployeePosition.Administrator);
    public const string HrAndProjectManagerPolicy = "HrAndProjectManager";
}