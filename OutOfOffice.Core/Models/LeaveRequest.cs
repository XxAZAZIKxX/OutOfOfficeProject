using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class LeaveRequest
{
    public ulong Id { get; set; }
    public Employee Employee { get; set; }
    public ulong EmployeeId { get; set; }
    public AbsenceReason AbsenceReason { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Comment { get; set; }
    public RequestStatus Status { get; set; }
}