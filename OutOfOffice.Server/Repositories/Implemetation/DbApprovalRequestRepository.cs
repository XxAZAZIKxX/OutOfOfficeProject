﻿using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implemetation;

/// <summary>
/// Implementation of <see cref="IApprovalRequestRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public class DbApprovalRequestRepository(DataContext dataContext) : IApprovalRequestRepository
{
    public async Task<ApprovalRequest?> GetApprovalRequestAsync(ulong requestId)
    {
        return await dataContext.ApprovalRequests.SingleOrDefaultAsync(p => p.Id == requestId);
    }

    public async Task<ApprovalRequest[]> GetApprovalRequstsAsync()
    {
        return await dataContext.ApprovalRequests.AsNoTracking().ToArrayAsync();
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
        var request = await dataContext.ApprovalRequests.SingleAsync(p=>p.Id == requestId);

        update(request);
        await dataContext.SaveChangesAsync();

        return request;
    }
}