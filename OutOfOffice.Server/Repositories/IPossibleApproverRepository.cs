using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface IPossibleApproverRepository
{
    Task<Employee[]> GetPossibleApproversForRequest(ulong requestId);
    Task AddApproversForRequestAsync(ulong requestId, IEnumerable<ulong> approverIds);
    Task DeleteApproversForRequestAsync(ulong requestId);
}