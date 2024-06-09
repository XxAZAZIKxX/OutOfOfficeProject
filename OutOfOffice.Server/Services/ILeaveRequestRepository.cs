using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Services;

public interface ILeaveRequestRepository
{
    LeaveRequest[] GetLeaveRequests();
    LeaveRequest[] GetLeaveRequestsOfEmployee(ulong employeeId);
    void UpdateLeaveRequest(LeaveRequest leaveRequest, Action<LeaveRequest> update);
    void AddLeaveRequest(LeaveRequest leaveRequest);
}