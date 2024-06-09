using StackExchange.Redis;

namespace OutOfOffice.Server.Services.Implemetation;

public class RedisRefreshTokenRepository(IConnectionMultiplexer redis) : IRefreshTokenRepository
{
    private readonly IDatabase _database = redis.GetDatabase(0);
    public void SetRefreshToken(ulong userId, string refreshToken, TimeSpan tokenLifetime)
    {
        _database.StringSet($"refreshToken:{userId}", refreshToken, tokenLifetime);
    }

    public bool IsRefreshTokenValid(ulong userId, string refreshToken)
    {
        RedisKey redisKey = $"refreshToken:{userId}";
        var value = _database.StringGet(redisKey);
        if (value.HasValue is false) return false;
        var isValid = value.ToString() == refreshToken;
        if (isValid is false) return false;
        _database.KeyDelete(redisKey);
        return true;
    }
}