using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Requests;

public class NewLeaveRequest
{
    public AbsenceReason Reason { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set;}
    public string? Comment { get; set; }
}