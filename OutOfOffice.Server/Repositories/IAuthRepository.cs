using OutOfOffice.Core.Models;
using OutOfOffice.Core.Requests;

namespace OutOfOffice.Server.Repositories;

public interface IAuthRepository
{
    Task<Employee?> IsUserCredentialsLegitAsync(AuthRequest credential);
}