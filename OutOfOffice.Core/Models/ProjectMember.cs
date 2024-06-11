using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OutOfOffice.Core.Models;

[Index(nameof(ProjectId), nameof(EmployeeId), IsUnique = true)]
public class ProjectMember
{
    [Key] public ulong Id { get; set; }
    [ForeignKey(nameof(Project))] public ulong ProjectId { get; set; }
    [Column] public Project Project { get; set; }
    [ForeignKey(nameof(Employee))] public ulong EmployeeId { get; set; }
    [Column] public Employee Employee { get; set; }
}