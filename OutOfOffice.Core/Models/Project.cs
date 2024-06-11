using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class Project
{
    [Key] public ulong Id { get; set; }
    [Column] public string ProjectName { get; set; }
    [Column] public ProjectType ProjectType { get; set; }
    [Column] public DateTimeOffset StartDate { get; set; }
    [Column] public DateTimeOffset? EndDate { get; set; }
    [Column] public Employee ProjectManager { get; set; }
    [Column] public string? Comment { get; set; }
    [Column] public ProjectStatus Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<ProjectMember> ProjectMembers { get; } = new List<ProjectMember>();


    public Project() { }
    public Project(Project other)
    {
        Id = other.Id;
        ProjectType = other.ProjectType;
        StartDate = other.StartDate;
        EndDate = other.EndDate;
        ProjectManager = other.ProjectManager;
        Comment = other.Comment;
        Status = other.Status;
    }
}