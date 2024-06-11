using OutOfOffice.Core.Exceptions.NotFound;
using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface ILeaveRequestRepository
{
    /// <summary>
    /// Get specified leave request
    /// </summary>
    /// <param name="requestId">The id of the leave request</param>
    /// <returns>
    /// The <see cref="LeaveRequest"/> if exists;
    /// otherwise <see langword="null"/>
    /// </returns>
    Task<LeaveRequest?> GetLeaveRequestAsync(ulong requestId);
    /// <summary>
    /// Get all leave requests from repository
    /// </summary>
    /// <returns>An array of <see cref="LeaveRequest"/></returns>
    Task<LeaveRequest[]> GetLeaveRequestsAsync();
    /// <summary>
    /// Get all leave requests from repository created by the specified employee
    /// </summary>
    /// <param name="employeeId">The id of the employee who created the leave requests</param>
    /// <returns>An array of <see cref="LeaveRequest"/></returns>
    Task<LeaveRequest[]> GetLeaveRequestsOfEmployeeAsync(ulong employeeId);
    /// <summary>
    /// Update specified leave request
    /// </summary>
    /// <param name="leaveRequestId">The id of the leave request to be updated</param>
    /// <param name="update">Action to be performed on the leave request</param>
    /// <returns>Updated leave request</returns>
    /// <exception cref="LeaveRequestNotFound"></exception>
    Task<LeaveRequest> UpdateLeaveRequestAsync(ulong leaveRequestId, Action<LeaveRequest> update);
    /// <summary>
    /// Add new leave request to the repository
    /// </summary>
    /// <param name="leaveRequest">Leave request to add</param>
    /// <returns>New request which was added</returns>
    Task<LeaveRequest> AddLeaveRequestAsync(LeaveRequest leaveRequest);
}