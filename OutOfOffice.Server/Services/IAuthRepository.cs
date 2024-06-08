using OutOfOffice.Core.Models;
using OutOfOffice.Core.Requests;

namespace OutOfOffice.Server.Services;

public interface IAuthRepository
{
    Employee? IsUserCredentialsLegit(AuthRequest credential);
}