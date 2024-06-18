using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;
using OutOfOffice.Server.Data.Models;

namespace OutOfOffice.Server.Repositories.Implementation;

public class DbPossibleApproverRepository(DataContext dataContext) : IPossibleApproverRepository
{
    public async Task<Employee[]> GetPossibleApproversForRequest(ulong requestId)
    {
        return await dataContext.PossibleApprovers
            .Include(p => p.Approver)
            .ThenInclude(p => p.PeoplePartner)
            .Where(p => p.ApprovalRequestId == requestId)
            .Select(p => p.Approver)
            .ToArrayAsync();
    }

    public async Task AddApproversForRequestAsync(ulong requestId, IEnumerable<ulong> approverIds)
    {
        var approvers = approverIds.Distinct()
            .Select(id => new PossibleApprover()
            {
                ApprovalRequestId = requestId,
                ApproverId = id
            });

        await dataContext.PossibleApprovers.AddRangeAsync(approvers);
        await dataContext.SaveChangesAsync();
    }

    public async Task DeleteApproversForRequestAsync(ulong requestId)
    {
        var approvers = dataContext.PossibleApprovers
            .Where(p => p.ApprovalRequestId == requestId);

        dataContext.PossibleApprovers.RemoveRange(approvers);
        await dataContext.SaveChangesAsync();
    }
}