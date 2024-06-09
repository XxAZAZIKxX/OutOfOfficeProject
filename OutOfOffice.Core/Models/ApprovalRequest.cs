using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class ApprovalRequest
{
    [Key] public ulong Id { get; set; }
    [Column] public Employee Approver { get; set; }
    [Column] public LeaveRequest LeaveRequest { get; set; }
    [Column] public RequestStatus Status { get; set; }
    [Column] public string? Comment { get; set; }
}