using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface IApprovalRequestRepository
{
    /// <summary>
    /// Get an approval request by its ID
    /// </summary>
    /// <param name="requestId">The ID of the approval request to get</param>
    /// <returns>
    /// The <see cref="ApprovalRequest"/> if found;
    /// otherwise <see langword="null"/>
    /// </returns>
    Task<ApprovalRequest?> GetApprovalRequestAsync(ulong requestId);

    /// <summary>
    /// Get all approval requests
    /// </summary>
    /// <returns>An array of approval request</returns>
    Task<ApprovalRequest[]> GetApprovalRequstsAsync();

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
    Task<ApprovalRequest> UpdateApprovalRequestAsync(ulong requestId, Action<ApprovalRequest> update);
}