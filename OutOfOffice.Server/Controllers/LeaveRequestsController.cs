using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Repositories;
using OutOfOffice.Server.Repositories.Implementation;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("/requests/leave/"), Authorize(Policy = Policies.EmployeePolicy)]
public class LeaveRequestsController(
    DbUnitOfWork dbUnitOfWork,
    ILeaveRequestRepository leaveRequestRepository,
    IApprovalRequestRepository approvalRequestRepository,
    IEmployeeRepository employeeRepository
    ) : ControllerBase
{
    [HttpGet, Route("get")]
    public async Task<IActionResult> GetLeaveRequests()
    {
        if (User.Claims.GetUserRole() == Policies.EmployeePolicy)
        {
            return Ok(await leaveRequestRepository.GetLeaveRequestsOfEmployeeAsync(User.Claims.GetUserId()));
        }
        return Ok(await leaveRequestRepository.GetLeaveRequestsAsync());
    }

    [HttpGet, Route("get/{id}")]
    public async Task<IActionResult> GetLeaveRequest([FromRoute] ulong id)
    {
        var employee = await employeeRepository.GetEmployeeAsync(id);
        if (employee is null) return NotFound($"Employee with id {id} not found!");

        var role = User.Claims.GetUserRole();
        var userId = User.Claims.GetUserId();

        if (role != Policies.EmployeePolicy) return Ok(employee);
        if (employee.Id != userId) return Forbid("You aren't allowed to do that!");

        return Ok(employee);
    }

    [HttpPost, Route("add")]
    public async Task<IActionResult> AddNewLeaveRequest([FromBody] NewLeaveRequest request)
    {
        var userId = User.Claims.GetUserId();
        await using var transaction = await dbUnitOfWork.BeginTransactionAsync();

        try
        {
            var employee = await employeeRepository.GetEmployeeAsync(userId);
            if (employee is null) return NotFound($"Employee with {userId} id not found!");
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

            await transaction.CommitAsync();
            return Ok(leaveRequest);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}