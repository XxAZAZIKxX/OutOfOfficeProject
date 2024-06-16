using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class Project
{
    public ulong Id { get; set; }
    public string ProjectName { get; set; }
    public ProjectType ProjectType { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public Employee ProjectManager { get; set; }
    public ulong ProjectManagerId { get; set; }
    public string? Comment { get; set; }
    public ProjectStatus Status { get; set; }
}