using StackExchange.Redis;

namespace OutOfOffice.Server.Config;

public class RedisConfiguration : BaseConfiguration
{
    public EndPointCollection EndPoints { get; } = new();

    public RedisConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("Redis");

        var endpoints = section.GetRequiredSection("EndPoints").Get<string[]?>() ??
                    throw GetSimpleMissingException("EndPoints");

        foreach (var endpoint in endpoints) EndPoints.Add(endpoint);
    }
}