using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Repositories;
using OutOfOffice.Server.Services;
using OutOfOffice.Server.Services.Implementation.NonInterfaceImpl;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("requests/approval/"), Authorize(Policy = Policies.HrAndProjectManagerPolicy)]
public sealed class ApprovalRequestsController(
    IApprovalRequestRepository approvalRequestRepository,
    IApprovalRequestService approvalRequestService,
    ExceptionHandlingService exceptionHandlingService
) : ControllerBase
{
    [HttpGet, Route("all")]
    public async Task<ActionResult<ApprovalRequest[]>> GetApprovalRequests()
    {
        return await approvalRequestRepository.GetApprovalRequestsAsync();
    }

    [HttpGet, Route("pending")]
    public async Task<ActionResult<ApprovalRequest[]>> GetPendingApprovalRequests()
    {
        var userId = User.Claims.GetUserId();
        return await approvalRequestRepository.GetPendingApprovalRequestsAsync(userId);
    }

[HttpGet, Route("{requestId}")]
    public async Task<ActionResult<ApprovalRequest>> GetApprovalRequest([FromRoute] ulong requestId)
    {
        var result = await approvalRequestRepository.GetApprovalRequestAsync(requestId);
        return result.Match(request => request, exceptionHandlingService.HandleException<ApprovalRequest>);
    }

    [HttpPost, Route("{requestId}/approve")]
    public async Task<ActionResult<ApprovalRequest>> ApproveRequest([FromRoute] ulong requestId, [FromQuery] string? comment = null)
    {
        var userId = User.Claims.GetUserId();

        var isCanChangeResult = await approvalRequestService.IsEmployeeCanChangeStatusAsync(requestId, userId);
        if (isCanChangeResult.IsFailed)
            return await exceptionHandlingService.HandleExceptionAsync(isCanChangeResult.Exception);

        if (isCanChangeResult.Value is false) return Forbid();

        var result = await approvalRequestService.ApproveRequestAsync(requestId, userId, comment ?? Optional<string>.None);
        return result.Match(request => request, exceptionHandlingService.HandleException<ApprovalRequest>);
    }

    [HttpPost, Route("{requestId}/reject")]
    public async Task<ActionResult<ApprovalRequest>> RejectRequest([FromRoute] ulong requestId,
        [FromQuery] string? comment = null)
    {
        var userId = User.Claims.GetUserId();

        var isCanChangeResult = await approvalRequestService.IsEmployeeCanChangeStatusAsync(requestId, userId);
        if (isCanChangeResult.IsFailed)
            return await exceptionHandlingService.HandleExceptionAsync(isCanChangeResult.Exception);

        if (isCanChangeResult.Value is false) return Forbid();

        var result = await approvalRequestService.RejectRequestAsync(requestId, userId, comment ?? Optional<string>.None);
        return result.Match(request => request, exceptionHandlingService.HandleException<ApprovalRequest>);
    }
}