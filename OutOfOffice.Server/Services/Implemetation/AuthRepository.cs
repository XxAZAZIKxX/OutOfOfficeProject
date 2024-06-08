using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Services.Implemetation;

public class AuthRepository(DataContext dataContext) : IAuthRepository
{
    public Employee? IsUserCredentialsLegit(AuthRequest auth)
    {
        var authCredentials = dataContext.AuthCredentials
            .AsNoTracking()
            .Include(p => p.Employee);

        var single = authCredentials.SingleOrDefault(p =>
            p.Username == auth.Username && p.PasswordHash == auth.PasswordHash);

        return single?.Employee;
    }
}