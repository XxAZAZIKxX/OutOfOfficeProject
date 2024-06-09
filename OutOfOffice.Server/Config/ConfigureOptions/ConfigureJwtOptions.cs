using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace OutOfOffice.Server.Config.ConfigureOptions;

public class ConfigureJwtOptions(JwtConfiguration configuration) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme) return;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidAudiences = configuration.Audiences,
            ValidIssuer = configuration.Issuer,
            IssuerSigningKey = configuration.GetSymmetricSecurityKey(),

            LifetimeValidator = (before, expires, token, parameters) =>
            {
                if (expires is null) return false;
                return DateTime.UtcNow.CompareTo(expires) <= 0;
            }
        };
    }
}