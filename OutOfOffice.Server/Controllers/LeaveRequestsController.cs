using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Repositories;
using OutOfOffice.Server.Repositories.Implemetation;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("/requests/leave/")]
public class LeaveRequestsController(
    DbUnitOfWork dbUnitOfWork,
    ILeaveRequestRepository leaveRequestRepository,
    IEmployeeRepository employeeRepository,
    ILogger<LeaveRequestsController> logger
    ) : ControllerBase
{
    [HttpGet, Route("get"), Authorize(Policy = Policies.EmployeePolicy)]
    public async Task<IActionResult> GetLeaveRequests()
    {
        if (User.Claims.GetUserRole() == Policies.EmployeePolicy)
        {
            return Ok(await leaveRequestRepository.GetLeaveRequestsOfEmployeeAsync(User.Claims.GetUserId()));
        }
        return Ok(await leaveRequestRepository.GetLeaveRequestsAsync());
    }

    [HttpPost, Route("add"), Authorize(Policy = Policies.EmployeePolicy)]
    public async Task<IActionResult> AddNewLeaveRequest([FromBody] NewLeaveRequest request)
    {
        var userId = User.Claims.GetUserId();
        var employee = await employeeRepository.GetEmployeeAsync(userId);
        if (employee is null) throw new Exception($"Employee was null! Employee id: {userId}");
        var leaveRequest = new LeaveRequest
        {
            AbsenceReason = request.Reason,
            Comment = request.Comment,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Employee = employee,
            Status = RequestStatus.New
        };
        await leaveRequestRepository.AddLeaveRequestAsync(leaveRequest);
        return Ok(leaveRequest);
    }

}