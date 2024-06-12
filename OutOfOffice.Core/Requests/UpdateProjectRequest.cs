using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Utilities;

namespace OutOfOffice.Core.Requests;

public class UpdateProjectRequest
{
    public Optional<string> ProjectName { get; set; }
    public Optional<ProjectType> ProjectType { get; set; }
    public Optional<DateTimeOffset> StartDate { get; set; }
    public Optional<string> Comment { get; set; }
}