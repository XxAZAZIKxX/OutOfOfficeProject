using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implemetation;

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

    public async Task UpdateLeaveRequestAsync(ulong leaveRequestId, Action<LeaveRequest> update)
    {
        var leaveRequest = await dataContext.LeaveRequests.SingleAsync(p=>p.Id == leaveRequestId);
        update(leaveRequest);
        await dataContext.SaveChangesAsync();
    }

    public async Task AddLeaveRequestAsync(LeaveRequest leaveRequest)
    {
        await dataContext.LeaveRequests.AddAsync(leaveRequest);
        await dataContext.SaveChangesAsync();
    }

    public async Task<LeaveRequest?> GetLeaveRequestAsync(ulong requestId)
    {
        return await dataContext.LeaveRequests.SingleOrDefaultAsync(p => p.Id == requestId);
    }
}