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
public sealed class LeaveRequestsController(
    DbUnitOfWork dbUnitOfWork,
    ILeaveRequestRepository leaveRequestRepository,
    IApprovalRequestRepository approvalRequestRepository,
    IEmployeeRepository employeeRepository
    ExceptionHandlingService exceptionHandlingService
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
        }, exceptionHandlingService.HandleException<LeaveRequest>);

            throw exception;
        });

    }

    [HttpPost, Route("add")]
    public async Task<ActionResult<LeaveRequest>> AddNewLeaveRequest([FromBody] NewLeaveRequest request)
    {
        var userId = User.Claims.GetUserId();
        await using var transaction = await dbUnitOfWork.BeginTransactionAsync();

        try
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

            var result = await approvalRequestService.AddApprovalRequestAsync(leaveRequest.Id);
            if (result.IsFailed) throw result.Exception;

            await transaction.CommitAsync();

            return leaveRequest;
        }
        catch(Exception e)
        {
            await transaction.RollbackAsync();
            return await exceptionHandlingService.HandleExceptionAsync(e);
        }
    }

    [HttpPost, Route("{requestId}/update")]
    public async Task<ActionResult<LeaveRequest>> UpdateLeaveRequest([FromRoute] ulong requestId,
        [FromBody] UpdateLeaveRequest updateLeaveRequest)
    {
        var userId = User.Claims.GetUserId();

        var leaveRequstResult = await leaveRequestRepository.GetLeaveRequestAsync(requestId);
        if (leaveRequstResult.IsFailed) return await exceptionHandlingService.HandleExceptionAsync(leaveRequstResult.Exception);
        var leaveRequest = leaveRequstResult.Value;
        if (leaveRequest.EmployeeId != userId) return Forbid();
        if (leaveRequest.Status != RequestStatus.New)
            return BadRequest("You can only update leave requests with a `New` status.");


        var result = await leaveRequestRepository.UpdateLeaveRequestAsync(requestId, request =>
        {
            if (updateLeaveRequest.AbsenceReason.HasValue)
                request.AbsenceReason = updateLeaveRequest.AbsenceReason.Value;

            if (updateLeaveRequest.StartDate.HasValue)
                request.StartDate = updateLeaveRequest.StartDate.Value;

            if (updateLeaveRequest.EndDate.HasValue)
                request.EndDate = updateLeaveRequest.EndDate.Value;

            if (updateLeaveRequest.Comment.HasValue)
                request.Comment = updateLeaveRequest.Comment.Value;
        });

        return result.Match(request => request, exceptionHandlingService.HandleException<LeaveRequest>);
    }
    }
}