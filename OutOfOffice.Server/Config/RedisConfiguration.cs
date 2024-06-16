using OutOfOffice.Server.Core.Extensions;
using StackExchange.Redis;

namespace OutOfOffice.Server.Config;

public class RedisConfiguration
{
    public EndPointCollection EndPoints { get; } = new();

    public RedisConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSectionOrThrow("Redis");

        var endpoints = section.GetOrThrow<string[]>("EndPoints");

        foreach (var endpoint in endpoints) EndPoints.Add(endpoint);
    }
}