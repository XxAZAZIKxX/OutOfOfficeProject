using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Services;
using OutOfOffice.Server.Services.Implemetation;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("/requests/leave/")]
public class LeaveRequestsController(
    DbUnitOfWork dbUnitOfWork,
    ILeaveRequestRepository leaveRequestRepository,
    ILogger<LeaveRequestsController> logger
    ) : ControllerBase
{
    [HttpGet, Authorize(Policy = Policies.EmployeePolicy)]
    public async Task<IActionResult> GetLeaveRequests()
    {
        return await Task.Run(new Func<IActionResult>(() =>
        {
            if (User.Claims.GetUserRole() == Policies.EmployeePolicy)
            {
                return Ok(leaveRequestRepository.GetLeaveRequestsOfEmployee(User.Claims.GetUserId()));
            }
            return Ok(leaveRequestRepository.GetLeaveRequests());
        }));
    }
}