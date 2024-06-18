using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;

namespace OutOfOffice.Server.Services;

public interface IApprovalRequestService
{
    /// <summary>
    /// Adds a new approval request and links approvers for this request
    /// </summary>
    /// <param name="leaveRequestId">ID of the leave request</param>
    /// <returns>
    /// The new added approval request
    /// </returns>
    /// <exception cref="EmployeeNotFoundException"></exception>
    /// <exception cref="LeaveRequestNotFoundException"></exception>
    Task<Result<ApprovalRequest>> AddApprovalRequestAsync(ulong leaveRequestId);

    /// <summary>
    /// Approves the specified request and cleans approvers for this request.
    /// Also decreases employee out of office balance
    /// </summary>
    /// <param name="approvalRequestId">ID of the approval request</param>
    /// <param name="approverId">ID of the approver</param>
    /// <param name="comment">Optional comment to the approving</param>
    /// <returns></returns>
    /// <exception cref="ApprovalRequestNotFoundException"></exception>
    /// <exception cref="EmployeeNotFoundException"></exception>
    /// <exception cref="LeaveRequestNotFoundException"></exception>
    Task<Result<ApprovalRequest>> ApproveRequestAsync(ulong approvalRequestId, ulong approverId,
        Optional<string> comment = default);

    /// <summary>
    /// Rejects the specified request and cleans approvers for this request
    /// </summary>
    /// <param name="approvalRequestId">ID of the approval request</param>
    /// <param name="employeeId">ID of the rejecter</param>
    /// <param name="comment">Optional comment to the rejecting</param>
    /// <returns></returns>
    /// <exception cref="ApprovalRequestNotFoundException"></exception>
    /// <exception cref="LeaveRequestNotFoundException"></exception>
    Task<Result<ApprovalRequest>> RejectRequestAsync(ulong approvalRequestId, ulong employeeId,
        Optional<string> comment = default);

    /// <summary>
    /// Cancels the approval request and cleans approvers to this request
    /// </summary>
    /// <param name="approvalRequestId">ID of the approval request</param>
    /// <returns></returns>
    /// <exception cref="ApprovalRequestNotFoundException"></exception>
    Task<Result<ApprovalRequest>> CancelApprovalRequestAsync(ulong approvalRequestId);

    /// <summary>
    /// Checks can user change status of the specified approval request
    /// </summary>
    /// <param name="approvalRequestId">ID of the approval request</param>
    /// <param name="employeeId">ID of the employee to check</param>
    /// <returns>
    /// <see langword="true"/> if can;
    /// otherwise <see langword="false"/>
    /// </returns>
    /// <exception cref="ApprovalRequestNotFoundException"></exception>
    Task<Result<bool>> IsEmployeeCanChangeStatusAsync(ulong approvalRequestId, ulong employeeId);
}