using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class Project
{
    [Key] public ulong Id { get; set; }
    [Column] public ProjectType ProjectType { get; set; }
    [Column] public DateTimeOffset StartDate { get; set; }
    [Column] public DateTimeOffset? EndDate { get; set; }
    [Column] public Employee ProjectManager { get; set; }
    [Column] public string? Comment { get; set; }
    [Column] public ProjectStatus Status { get; set; }
}