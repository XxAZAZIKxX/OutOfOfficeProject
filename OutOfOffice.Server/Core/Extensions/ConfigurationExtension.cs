using OutOfOffice.Server.Core.Exceptions;

namespace OutOfOffice.Server.Core.Extensions;

public static class ConfigurationExtension
{
    public static IConfigurationSection GetSectionOrThrow(this IConfiguration configuration, string key)
    {
        var resultSection = configuration.GetSection(key);
        if (resultSection.Exists()) return resultSection;

        throw new ConfigurationMissingException($"Section `{resultSection.Path}` is missing in configuration");
    }

    public static TValue GetOrThrow<TValue>(this IConfiguration configuration, string key)
    {
        return configuration.GetSectionOrThrow(key).GetOrThrow<TValue>();
    }

    public static TValue GetOrThrow<TValue>(this IConfigurationSection section)
    {
        var value = section.Get<TValue>();
        if (value is not null) return value;

        throw new ConfigurationMissingException($"Value for `{section.Path}` is missing in configuration section");
    }
}