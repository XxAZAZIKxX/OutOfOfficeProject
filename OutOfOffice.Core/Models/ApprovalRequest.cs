using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class ApprovalRequest
{
    public ulong Id { get; set; }
    public Employee? Approver { get; set; }
    public ulong? ApproverId { get; set; }
    public LeaveRequest LeaveRequest { get; set; }
    public ulong LeaveRequestId { get; set; }
    public RequestStatus Status { get; set; }
    public string? Comment { get; set; }
}