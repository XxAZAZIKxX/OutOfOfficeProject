using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest[]> GetLeaveRequestsAsync();
    Task<LeaveRequest[]> GetLeaveRequestsOfEmployeeAsync(ulong employeeId);
    Task UpdateLeaveRequestAsync(ulong leaveRequestId, Action<LeaveRequest> update);
    Task AddLeaveRequestAsync(LeaveRequest leaveRequest);
    Task<LeaveRequest?> GetLeaveRequestAsync(ulong requestId);
}