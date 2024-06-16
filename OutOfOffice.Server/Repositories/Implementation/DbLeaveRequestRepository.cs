using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="ILeaveRequestRepository"/> which using <see cref="DataContext"/>
/// </summary>
public class DbLeaveRequestRepository(DataContext dataContext) : ILeaveRequestRepository
{
    public async Task<Result<LeaveRequest>> GetLeaveRequestAsync(ulong requestId)
    {
        var singleOrDefault = await dataContext.LeaveRequests
            .Include(p => p.Employee)
            .SingleOrDefaultAsync(p => p.Id == requestId);

        if (singleOrDefault is null)
            return new LeaveRequestNotFound($"Leave request with id `{requestId}` not found!");

        return singleOrDefault;
    }

    public async Task<LeaveRequest[]> GetLeaveRequestsAsync()
    {
        return await dataContext.LeaveRequests.AsNoTracking().Include(p => p.Employee).ToArrayAsync();
    }

    public async Task<LeaveRequest[]> GetLeaveRequestsOfEmployeeAsync(ulong employeeId)
    {
        return await dataContext.LeaveRequests.AsNoTracking()
            .Include(p => p.Employee)
            .Where(p => p.Employee.Id == employeeId)
            .ToArrayAsync();
    }

    public async Task<Result<LeaveRequest>> UpdateLeaveRequestAsync(ulong leaveRequestId, Action<LeaveRequest> update)
    {
        var leaveRequest = await GetLeaveRequestAsync(leaveRequestId);
        if (leaveRequest.IsFailed) return leaveRequest;

        update(leaveRequest.Value);
        await dataContext.SaveChangesAsync();

        return leaveRequest;
    }

    public async Task<LeaveRequest> AddLeaveRequestAsync(LeaveRequest leaveRequest)
    {
        await dataContext.LeaveRequests.AddAsync(leaveRequest);
        await dataContext.SaveChangesAsync();
        return leaveRequest;
    }
}