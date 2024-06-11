using OutOfOffice.Core.Models;
using OutOfOffice.Core.Requests;

namespace OutOfOffice.Server.Repositories;

public interface IAuthRepository
{
    /// <summary>
    /// Validates the <see cref="credential"/> and returns an <see cref="Employee"/>
    /// when <see cref="credential"/> is correct
    /// </summary>
    /// <param name="credential">User credential to check</param>
    /// <returns>
    /// The <see cref="Employee"/> when validation is succesful;
    /// otherwise <see langword="null"/>
    /// </returns>
    Task<Employee?> IsUserCredentialsLegitAsync(AuthRequest credential);
}