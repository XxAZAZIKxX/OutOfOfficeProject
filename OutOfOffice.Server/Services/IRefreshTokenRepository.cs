namespace OutOfOffice.Server.Services;

public interface IRefreshTokenRepository
{
    void SetRefreshToken(ulong userId, string refreshToken, TimeSpan tokenLifetime);
    bool IsRefreshTokenValid(ulong userId, string refreshToken);
}