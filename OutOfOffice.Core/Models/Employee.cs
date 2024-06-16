using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class Employee
{
    public ulong Id { get; set; }
    public string FullName { get; set; }
    public EmployeeSubdivision Subdivision { get; set; }
    public EmployeePosition Position { get; set; }
    public EmployeeStatus Status { get; set; }
    public Employee? PeoplePartner { get; set; }
    public ulong? PeoplePartnerId { get; set; }
    public int OutOfOfficeBalance { get; set; }
    public byte[]? PhotoBytes { get; set; }
}