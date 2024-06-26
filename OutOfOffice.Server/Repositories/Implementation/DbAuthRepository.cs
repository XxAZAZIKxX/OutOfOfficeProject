﻿using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation if <see cref="IAuthRepository"/> which uses <see cref="DataContext"/>
/// </summary>
public sealed class DbAuthRepository(DataContext dataContext) : IAuthRepository
{
    public async Task<Employee?> IsUserCredentialsLegitAsync(string username, string passwordHash)
    {
        var authCredentials = dataContext.AuthCredentials
            .Include(p => p.Employee);

        var single = await authCredentials.SingleOrDefaultAsync(p =>
            p.Username == username && p.PasswordHash == passwordHash);

        return single?.Employee;
    }
}