using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OutOfOffice.Core.Models.Enums;

namespace OutOfOffice.Core.Models;

public class LeaveRequest
{
    [Key] public ulong Id { get; set; }
    [Column] public Employee Employee { get; set; }
    [Column] public AbsenceReason AbsenceReason { get; set; }
    [Column] public DateTimeOffset StartDate { get; set; }
    [Column] public DateTimeOffset EndDate { get; set; }
    [Column] public string? Comment { get; set; }
    [Column] public RequestStatus Status { get; set; } = RequestStatus.New;

    public LeaveRequest() { }

    public LeaveRequest(LeaveRequest other) 
    {
        Id = other.Id;
        Employee = other.Employee;
        AbsenceReason = other.AbsenceReason;
        StartDate = other.StartDate;
        EndDate = other.EndDate;
        Comment = other.Comment;
        Status = other.Status;
    }

}