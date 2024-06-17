using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Exceptions.NotFound;
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
    [HttpGet, Route("all")]
    public async Task<ActionResult<LeaveRequest[]>> GetLeaveRequests()
    {
        if (User.Claims.GetUserRole() == Policies.EmployeePolicy)
        {
            return await leaveRequestRepository.GetLeaveRequestsOfEmployeeAsync(User.Claims.GetUserId());
        }
        return await leaveRequestRepository.GetLeaveRequestsAsync();
    }

    [HttpGet, Route("{projectId}")]
    public async Task<ActionResult<LeaveRequest>> GetLeaveRequest([FromRoute] ulong projectId)
    {
        var leaveRequestResult = await leaveRequestRepository.GetLeaveRequestAsync(projectId);

        return leaveRequestResult.Match<ActionResult<LeaveRequest>>(request =>
        {
            var userId = User.Claims.GetUserId();
            var role = User.Claims.GetUserRole();

            if (role is Policies.EmployeePolicy && request.Employee.Id != userId)
                return Forbid();

            return request;
        }, exception =>
        {
            if (exception is LeaveRequestNotFound)
                return NotFound(exception.Message);

            throw exception;
        });

    }

    [HttpPost, Route("add")]
    public async Task<ActionResult<LeaveRequest>> AddNewLeaveRequest([FromBody] NewLeaveRequest request)
    {
        var userId = User.Claims.GetUserId();
        await using var transaction = await dbUnitOfWork.BeginTransactionAsync();

        var employeeResult = await employeeRepository.GetEmployeeAsync(userId);
        return await employeeResult.Match<Task<ActionResult<LeaveRequest>>>(async employee =>
        {
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

            return leaveRequest;
        }, async exception =>
        {
            await transaction.RollbackAsync();

            if (exception is EmployeeNotFoundException)
                return NotFound(exception.Message);

            throw exception;
        });
    }
}