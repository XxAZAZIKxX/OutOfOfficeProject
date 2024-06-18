using StackExchange.Redis;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Implementation of <see cref="IRefreshTokenRepository"/> which uses RedisDatabase
/// </summary>
public sealed class RedisRefreshTokenRepository(IConnectionMultiplexer redis) : IRefreshTokenRepository
{
    private readonly IDatabase _database = redis.GetDatabase(0);

    public async Task SetRefreshTokenAsync(ulong userId, string refreshToken, TimeSpan tokenLifetime)
    {
        RedisKey redisKey = $"refreshToken:{userId}";
        await _database.StringSetAsync(redisKey, refreshToken, tokenLifetime);
    }

    public async Task<bool> IsRefreshTokenValidAsync(ulong userId, string refreshToken)
    {
        RedisKey redisKey = $"refreshToken:{userId}";
        var value = await _database.StringGetAsync(redisKey);
        if (value.HasValue is false) return false;
        var isValid = value.ToString() == refreshToken;
        if (isValid is false) return false;
        await _database.KeyDeleteAsync(redisKey);
        return true;
    }
}