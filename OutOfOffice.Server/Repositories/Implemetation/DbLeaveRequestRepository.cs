using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implemetation;

/// <summary>
/// Implementation of <see cref="ILeaveRequestRepository"/> which using <see cref="DataContext"/>
/// </summary>
public class DbLeaveRequestRepository(DataContext dataContext) : ILeaveRequestRepository
{
    public async Task<LeaveRequest[]> GetLeaveRequestsAsync()
    {
        return await dataContext.LeaveRequests
            .AsNoTracking()
            .Include(p => p.Employee).ToArrayAsync();
    }

    public async Task<LeaveRequest[]> GetLeaveRequestsOfEmployeeAsync(ulong employeeId)
    {
        return await dataContext.LeaveRequests
            .AsNoTracking()
            .Include(p => p.Employee)
            .Where(p => p.Employee.Id == employeeId)
            .ToArrayAsync();
    }

    public async Task<LeaveRequest> UpdateLeaveRequestAsync(ulong leaveRequestId, Action<LeaveRequest> update)
    {
        var leaveRequest = await dataContext.LeaveRequests.SingleAsync(p=>p.Id == leaveRequestId);
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

    public async Task<LeaveRequest?> GetLeaveRequestAsync(ulong requestId)
    {
        return await dataContext.LeaveRequests.SingleOrDefaultAsync(p => p.Id == requestId);
    }
}