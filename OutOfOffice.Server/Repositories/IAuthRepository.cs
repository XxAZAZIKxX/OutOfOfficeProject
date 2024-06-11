using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Repositories;

public interface IAuthRepository
{
    /// <summary>
    /// Validates the user credential and returns an <see cref="Employee"/>
    /// when user credential is correct
    /// </summary>
    /// <param name="username">Unique employee login name</param>
    /// <param name="paswordHash">Hash of user password </param>
    /// <returns>
    /// The <see cref="Employee"/> when validation is succesful;
    /// otherwise <see langword="null"/>
    /// </returns>
    Task<Employee?> IsUserCredentialsLegitAsync(string username, string paswordHash);
}