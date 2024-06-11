using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Utilities;

namespace OutOfOffice.Core.Requests;

public class UpdateProjectRequest
{
    public Optional<string> ProjectName { get; set; } = Optional.None;
    public Optional<ProjectType> ProjectType { get; set; } = Optional.None;
    public Optional<DateTimeOffset> StartDate { get; set; } = Optional.None;
    public Optional<string> Comment { get; set; } = Optional.None;
}