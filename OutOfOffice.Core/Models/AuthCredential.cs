using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OutOfOffice.Core.Models;

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(EmployeeId), IsUnique = true)]
public class AuthCredential
{
    [Key] public ulong Id { get; set; }
    [Column] [StringLength(25)] public string Username { get; set; }
    [ForeignKey(nameof(Employee))] public ulong EmployeeId { get; set; }
    [Column] public Employee Employee { get; set; }
    [Column] public string PasswordHash { get; set; }
}