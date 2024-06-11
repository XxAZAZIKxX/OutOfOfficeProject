using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implemetation;

/// <summary>
/// Implementation if <see cref="IAuthRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public class DbAuthRepository(DataContext dataContext) : IAuthRepository
{
    public async Task<Employee?> IsUserCredentialsLegitAsync(AuthRequest auth)
    {
        var authCredentials = dataContext.AuthCredentials
            .AsNoTracking()
            .Include(p => p.Employee);

        var single = await authCredentials.SingleOrDefaultAsync(p =>
            p.Username == auth.Username && p.PasswordHash == auth.PasswordHash);

        return single?.Employee;
    }
}