using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Requests;

public class NewProjectRequest
{
    public string ProjectName { get; set; }
    public ProjectType ProjectType { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Comment { get; set; }
}