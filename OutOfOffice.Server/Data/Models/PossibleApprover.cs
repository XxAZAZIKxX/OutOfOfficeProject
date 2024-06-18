using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Data.Models;

[Index(nameof(ApprovalRequestId), nameof(ApproverId), IsUnique = true)]
public class PossibleApprover
{
    public ulong Id { get; set; }
    public ApprovalRequest ApprovalRequest { get; set; }
    [ForeignKey(nameof(ApprovalRequest))] public ulong ApprovalRequestId { get; set; }
    public Employee Approver { get; set; }
    [ForeignKey(nameof(Approver))] public ulong ApproverId { get; set; }
}