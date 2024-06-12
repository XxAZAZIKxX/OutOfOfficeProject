using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IApprovalRequestRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public class DbApprovalRequestRepository(DataContext dataContext) : IApprovalRequestRepository
{
    public async Task<ApprovalRequest?> GetApprovalRequestAsync(ulong requestId)
    {
        return await dataContext.ApprovalRequests
            .Include(p=>p.Approver)
            .Include(p=>p.LeaveRequest)
            .ThenInclude(p=>p.Employee)
            .SingleOrDefaultAsync(p => p.Id == requestId);
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
        var result = new ApprovalRequest(request);

        await dataContext.ApprovalRequests.AddAsync(result);
        await dataContext.SaveChangesAsync();

        return result;
    }

    public async Task<ApprovalRequest> UpdateApprovalRequestAsync(ulong requestId, Action<ApprovalRequest> update)
    {
        var request = await GetApprovalRequestAsync(requestId) ??
                      throw new ApprovalRequestNotFound($"Approval request with id {requestId} doesnt exists");

        update(request);
        await dataContext.SaveChangesAsync();

        return request;
    }
}