using OutOfOffice.Core.Models;
using OutOfOffice.Core.Models.Enums;
using OutOfOffice.Core.Utilities;
using OutOfOffice.Server.Repositories;

namespace OutOfOffice.Server.Services.Implementation;

public class ApprovalRequestService(
    IProjectRepository projectRepository,
    IEmployeeRepository employeeRepository,
    IApprovalRequestRepository approvalRequestRepository,
    IPossibleApproverRepository possibleApproverRepository,
    ILeaveRequestRepository leaveRequestRepository
    ) : IApprovalRequestService
{
    public async Task<Result<ApprovalRequest>> AddApprovalRequestAsync(ulong leaveRequestId)
    {
        var possibleApproverIds = new List<ulong>();

        var leaveRequestResult = await leaveRequestRepository.GetLeaveRequestAsync(leaveRequestId);
        if (leaveRequestResult.IsFailed) return leaveRequestResult.Exception;

        var leaveRequest = leaveRequestResult.Value;

        var employeeId = leaveRequest.EmployeeId;
        var projectsResult = await projectRepository.GetProjectsWithEmployeeAsync(employeeId);
        if (projectsResult.IsFailed) return projectsResult.Exception;

        var managerIds = projectsResult.Value.Select(p => p.ProjectManagerId).ToArray();

        possibleApproverIds.AddRange(managerIds);

        var employeeResult = await employeeRepository.GetEmployeeAsync(employeeId);
        if (employeeResult.IsFailed) return employeeResult.Exception;

        if (employeeResult.Value.PeoplePartnerId is { } id) possibleApproverIds.Add(id);

        var approvalRequest = await approvalRequestRepository.AddApprovalRequestAsync(new ApprovalRequest()
        {
            LeaveRequest = leaveRequest,
            Status = RequestStatus.New
        });
        possibleApproverIds.Remove(leaveRequest.EmployeeId);
        await possibleApproverRepository.AddApproversForRequestAsync(approvalRequest.Id, possibleApproverIds);

        return approvalRequest;
    }

    public async Task<Result<ApprovalRequest>> ApproveRequestAsync(ulong approvalRequestId, ulong approverId,
        Optional<string> comment = default)
    {
        // Update approval
        var updateApprovalRequestResult = await approvalRequestRepository.UpdateApprovalRequestAsync(approvalRequestId, request =>
        {
            if (comment.HasValue) request.Comment = comment.Value;
            request.ApproverId = approverId;
            request.Status = RequestStatus.Approved;
        });
        if (updateApprovalRequestResult.IsFailed) return updateApprovalRequestResult.Exception;
        var approvalRequest = updateApprovalRequestResult.Value;

        // Update leave
        var updateLeaveRequestResult = await leaveRequestRepository.UpdateLeaveRequestAsync(
            approvalRequest.LeaveRequestId, request =>
        {
            request.Status = RequestStatus.Approved;
        });
        if (updateLeaveRequestResult.IsFailed) return updateLeaveRequestResult.Exception;
        var leaveRequest = updateLeaveRequestResult.Value;

        // Update possible approvers
        await possibleApproverRepository.DeleteApproversForRequestAsync(approvalRequestId);

        // Update employee
        var updateEmployeeResult = await employeeRepository.UpdateEmployeeAsync(leaveRequest.EmployeeId, employee =>
        {
            var days = leaveRequest.EndDate.Subtract(leaveRequest.StartDate).TotalDays;
            employee.OutOfOfficeBalance -= (int)days;
        });
        if (updateEmployeeResult.IsFailed) return updateEmployeeResult.Exception;

        return approvalRequest;
    }

    public async Task<Result<ApprovalRequest>> RejectRequestAsync(ulong approvalRequestId, ulong employeeId,
        Optional<string> comment = default)
    {
        // Update approval
        var updateApprovalRequestResult = await approvalRequestRepository.UpdateApprovalRequestAsync(approvalRequestId, request =>
        {
            if (comment.HasValue) request.Comment = comment.Value;
            request.ApproverId = employeeId;
            request.Status = RequestStatus.Rejected;
        });
        if (updateApprovalRequestResult.IsFailed) return updateApprovalRequestResult.Exception;
        var approvalRequest = updateApprovalRequestResult.Value;

        var leaveRequestResult = await leaveRequestRepository.UpdateLeaveRequestAsync(approvalRequest.LeaveRequestId,
            request =>
            {
                request.Status = RequestStatus.Rejected;
            });
        if (leaveRequestResult.IsFailed) return leaveRequestResult.Exception;

        // Update possible approvers
        await possibleApproverRepository.DeleteApproversForRequestAsync(approvalRequestId);

        return approvalRequest;
    }

    public async Task<Result<ApprovalRequest>> CancelApprovalRequestAsync(ulong approvalRequestId)
    {
        var updateResult = await approvalRequestRepository.UpdateApprovalRequestAsync(approvalRequestId, request =>
        {
            request.Status = RequestStatus.Cancelled;
        });
        if (updateResult.IsFailed) return updateResult.Exception;

        await possibleApproverRepository.DeleteApproversForRequestAsync(approvalRequestId);
        return updateResult.Value;
    }

    public async Task<Result<bool>> IsEmployeeCanChangeStatusAsync(ulong approvalRequestId, ulong employeeId)
    {
        var approvalRequestResult = await approvalRequestRepository.GetApprovalRequestAsync(approvalRequestId);
        if (approvalRequestResult.IsFailed) return approvalRequestResult.Exception;

        var approvers = await possibleApproverRepository.GetPossibleApproversForRequest(approvalRequestId);
        return approvers.Any(p => p.Id == employeeId);
    }
}