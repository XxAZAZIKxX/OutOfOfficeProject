using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;

namespace OutOfOffice.Server.Repositories;

public interface IApprovalRequestRepository
{
    /// <summary>
    /// Get all approval requests
    /// </summary>
    /// <returns>An array of approval request</returns>
    Task<ApprovalRequest[]> GetApprovalRequestsAsync();

    /// <summary>
    /// Get an approval request by its ID
    /// </summary>
    /// <param name="approvalRequestId">The ID of the approval request to get</param>
    /// <returns>
    /// The <see cref="ApprovalRequest"/> if found;
    /// otherwise <see langword="null"/>
    /// </returns>
    /// <exception cref="ApprovalRequestNotFoundException"></exception>
    Task<Result<ApprovalRequest>> GetApprovalRequestAsync(ulong approvalRequestId);

    /// <summary>
    /// Get correspoding approval request to the spectified leave request
    /// </summary>
    /// <param name="leaveRequestId">ID of the leave request</param>
    /// <returns></returns>
    /// <exception cref="ApprovalRequestNotFoundException"></exception>
    Task<Result<ApprovalRequest>> GetApprovalRequestByLeaveRequestAsync(ulong leaveRequestId);

    /// <summary>
    /// Get the pending approval requests for the specified approver
    /// </summary>
    /// <param name="approverId">ID of the approver</param>
    /// <returns></returns>
    Task<ApprovalRequest[]> GetPendingApprovalRequestsAsync(ulong approverId);

    /// <summary>
    /// Adds a new approval request
    /// </summary>
    /// <param name="request">The approval request to add</param>
    /// <returns>The new approval request which was added</returns>
    Task<ApprovalRequest> AddApprovalRequestAsync(ApprovalRequest request);

    /// <summary>
    /// Updates an approval request
    /// </summary>
    /// <param name="requestId">The ID of the approval request to update</param>
    /// <param name="update">An action to perform on the approval request</param>
    /// <returns>The updated approval request</returns>
    /// <exception cref="ApprovalRequestNotFoundException"></exception>
    Task<Result<ApprovalRequest>> UpdateApprovalRequestAsync(ulong requestId, Action<ApprovalRequest> update);
}