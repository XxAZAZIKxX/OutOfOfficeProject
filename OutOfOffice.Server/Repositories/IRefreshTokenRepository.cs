﻿namespace OutOfOffice.Server.Repositories;

public interface IRefreshTokenRepository
{
    Task SetRefreshTokenAsync(ulong userId, string refreshToken, TimeSpan tokenLifetime);
    Task<bool> IsRefreshTokenValidAsync(ulong userId, string refreshToken);
}