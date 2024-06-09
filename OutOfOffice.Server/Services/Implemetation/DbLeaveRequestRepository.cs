using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Services.Implemetation;

public class DbLeaveRequestRepository(DataContext dataContext) : ILeaveRequestRepository
{
    public LeaveRequest[] GetLeaveRequests()
    {
        var leaveRequests = dataContext.LeaveRequests.AsNoTracking()
            .Include(p => p.Employee)
            .ToArray();
        return leaveRequests;
    }

    public LeaveRequest[] GetLeaveRequestsOfEmployee(ulong employeeId)
    {
        return dataContext.LeaveRequests.AsNoTracking()
            .Include(p => p.Employee)
            .Where(p => p.Employee.Id == employeeId)
            .ToArray();
    }

    public void UpdateLeaveRequest(LeaveRequest leaveRequest, Action<LeaveRequest> update)
    {
        var id = leaveRequest.Id;
        var single = dataContext.LeaveRequests.SingleOrDefault(p => p.Id == id);
        if (single == null) return;
        update(single);
        single.Id = id;
        dataContext.SaveChanges();
    }

    public void AddLeaveRequest(LeaveRequest leaveRequest)
    {
        dataContext.LeaveRequests.Add(leaveRequest);
        dataContext.SaveChanges();
    }
}