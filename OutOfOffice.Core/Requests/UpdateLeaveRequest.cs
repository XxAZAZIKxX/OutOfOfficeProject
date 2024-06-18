using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Utilities;

namespace OutOfOffice.Core.Requests;

public class UpdateLeaveRequest
{
    public Optional<AbsenceReason> AbsenceReason { get; set; }
    public Optional<DateTimeOffset> StartDate { get; set; }
    public Optional<DateTimeOffset> EndDate { get; set; }
    public Optional<string> Comment { get; set; }
}