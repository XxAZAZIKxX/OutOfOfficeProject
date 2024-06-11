using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="ILeaveRequestRepository"/> which using <see cref="DataContext"/>
/// </summary>
public class DbLeaveRequestRepository(DataContext dataContext) : ILeaveRequestRepository
{
    public async Task<LeaveRequest?> GetLeaveRequestAsync(ulong requestId)
    {
        return await dataContext.LeaveRequests.SingleOrDefaultAsync(p => p.Id == requestId);
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

    public async Task<LeaveRequest> UpdateLeaveRequestAsync(ulong leaveRequestId, Action<LeaveRequest> update)
    {
        var leaveRequest = await GetLeaveRequestAsync(leaveRequestId) ??
                           throw new LeaveRequestNotFound($"Leave request with {leaveRequestId} doesnt exists");

        update(leaveRequest);
        await dataContext.SaveChangesAsync();

        return leaveRequest;
    }

    public async Task<LeaveRequest> AddLeaveRequestAsync(LeaveRequest leaveRequest)
    {
        var result = new LeaveRequest(leaveRequest);
        await dataContext.LeaveRequests.AddAsync(result);
        await dataContext.SaveChangesAsync();
        return result;
    }
}