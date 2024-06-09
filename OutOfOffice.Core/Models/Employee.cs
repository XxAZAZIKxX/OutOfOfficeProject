using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class Employee
{
    [Key] public ulong Id { get; set; }
    [Column] public string FullName { get; set; }
    [Column] public EmployeeSubdivision Subdivision { get; set; }
    [Column] public EmployeePosition Position { get; set; }
    [Column] public EmployeeStatus Status { get; set; }

    [Column]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Employee? PeoplePartner { get; set; }

    [Column] public int OutOfOfficeBalance { get; set; }
    [Column] public byte[]? PhotoBytes { get; set; }
}