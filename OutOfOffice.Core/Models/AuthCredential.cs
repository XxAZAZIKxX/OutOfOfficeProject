using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OutOfOffice.Core.Models;

[Index(nameof(Employee), IsUnique = true)]
public class AuthCredential
{
    [Key] public ulong Id { get; set; }
    [Column] public Employee Employee { get; set; }
    [Column] public string PasswordHash { get; set; }
}