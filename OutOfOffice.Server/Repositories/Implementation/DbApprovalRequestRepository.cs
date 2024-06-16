using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IApprovalRequestRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public class DbApprovalRequestRepository(DataContext dataContext) : IApprovalRequestRepository
{
    public async Task<Result<ApprovalRequest>> GetApprovalRequestAsync(ulong requestId)
    {
        var singleOrDefault = await dataContext.ApprovalRequests
            .Include(p=>p.Approver)
            .Include(p=>p.LeaveRequest)
            .ThenInclude(p=>p.Employee)
            .SingleOrDefaultAsync(p => p.Id == requestId);

        if (singleOrDefault == null) 
            return new ApprovalRequestNotFound($"Approval request with id `{requestId}` not found");
        return singleOrDefault;
    }

    public async Task<ApprovalRequest[]> GetApprovalRequstsAsync()
    {
        return await dataContext.ApprovalRequests.AsNoTracking()
            .Include(p=>p.Approver)
            .Include(p=>p.LeaveRequest)
            .ThenInclude(p=>p.Employee)
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