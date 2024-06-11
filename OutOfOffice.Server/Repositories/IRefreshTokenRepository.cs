namespace OutOfOffice.Server.Repositories;

public interface IRefreshTokenRepository
{
    /// <summary>
    /// Sets refresh token of user in repository
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="refreshToken">Refresh token of user</param>
    /// <param name="tokenLifetime">The time the token will live in repository</param>
    /// <returns></returns>
    Task SetRefreshTokenAsync(ulong userId, string refreshToken, TimeSpan tokenLifetime);
    /// <summary>
    /// Validated refresh token of user
    /// </summary>
    /// <param name="userId">ID user which ownes the <see cref="refreshToken"/></param>
    /// <param name="refreshToken">Refresh token of user</param>
    /// <returns>
    /// <see langword="true"/> if validation succesful;
    /// otherwise <see langword="false"/> 
    /// </returns>
    Task<bool> IsRefreshTokenValidAsync(ulong userId, string refreshToken);
}