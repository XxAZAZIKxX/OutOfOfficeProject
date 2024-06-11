using OutOfOffice.Core.Models;
using OutOfOffice.Core.Requests;

namespace OutOfOffice.Server.Services;

public interface IAuthRepository
{
    Task<Employee?> IsUserCredentialsLegitAsync(AuthRequest credential);
}