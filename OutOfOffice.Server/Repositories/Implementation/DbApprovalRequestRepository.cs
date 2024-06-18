using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IApprovalRequestRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public sealed class DbApprovalRequestRepository(DataContext dataContext) : IApprovalRequestRepository
{
    public async Task<Result<ApprovalRequest>> GetApprovalRequestAsync(ulong approvalRequestId)
    {
        var singleOrDefault = await dataContext.ApprovalRequests
            .Include(p=>p.Approver)
            .Include(p=>p.LeaveRequest)
            .ThenInclude(p=>p.Employee)
            .SingleOrDefaultAsync(p => p.Id == approvalRequestId);

        if (singleOrDefault == null) 
            return new ApprovalRequestNotFoundException($"Approval request with id `{approvalRequestId}` not found");
        return singleOrDefault;
    }

    public async Task<Result<ApprovalRequest>> GetApprovalRequestByLeaveRequestAsync(ulong leaveRequestId)
    {
        var singleOrDefault = await dataContext.ApprovalRequests
            .Include(p => p.Approver)
            .Include(p => p.LeaveRequest)
            .ThenInclude(p => p.Employee)
            .SingleOrDefaultAsync(p => p.LeaveRequestId == leaveRequestId);

        if (singleOrDefault is null)
            return new ApprovalRequestNotFoundException(
                $"Approval request with leave request id `{leaveRequestId}` not found!");

        return singleOrDefault;
    }

    public async Task<ApprovalRequest[]> GetApprovalRequestsAsync()
    {
        return await dataContext.ApprovalRequests.AsNoTracking()
            .Include(p=>p.Approver)
            .Include(p=>p.LeaveRequest)
            .ThenInclude(p=>p.Employee)
            .ToArrayAsync();
    }

    public async Task<ApprovalRequest[]> GetPendingApprovalRequestsAsync(ulong approverId)
    {
        return await dataContext.PossibleApprovers
            .Include(p=>p.ApprovalRequest)
            .ThenInclude(p=>p.LeaveRequest)
            .Where(p=>p.ApproverId == approverId)
            .Select(p=>p.ApprovalRequest)
            .ToArrayAsync();
    }

    public async Task<ApprovalRequest> AddApprovalRequestAsync(ApprovalRequest request)
    {
        await dataContext.ApprovalRequests.AddAsync(request);
        await dataContext.SaveChangesAsync();

        return request;
    }

    public async Task<Result<ApprovalRequest>> UpdateApprovalRequestAsync(ulong requestId, Action<ApprovalRequest> update)
    {
        var request = await GetApprovalRequestAsync(requestId);
        if (request.IsFailed) return request;

        update(request.Value);
        await dataContext.SaveChangesAsync();

        return request;
    }
}