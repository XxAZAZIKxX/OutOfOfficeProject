using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OutOfOffice.Server.Core.Extensions;

namespace OutOfOffice.Server.Config;

public class JwtConfiguration : BaseConfiguration
{
    public string Issuer { get; }
    public TimeSpan TokenLifetime { get; }
    public TimeSpan RefreshTokenLifetime { get; }
    public string[] Audiences { get; }

    public TokenValidationParameters ValidationParameters
    {
        get
        {
            return new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidAudiences = Audiences,
                ValidIssuer = Issuer,
                IssuerSigningKey = GetSymmetricSecurityKey(),

                LifetimeValidator = (before, expires, token, parameters) =>
                {
                    if (expires is null) return false;
                    return DateTime.UtcNow.CompareTo(expires) <= 0;
                }
            };
        }
    }

    private readonly byte[] _secret;

    public JwtConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSectionOrThrow("Jwt");
        Issuer = section.GetOrThrow<string>("Issuer");

        var secretString = section.GetOrThrow<string>("Secret");
        _secret = Encoding.UTF8.GetBytes(secretString);

        var tokenLifetimeNumber = section.GetOrThrow<ulong>("TokenLifetime");
        TokenLifetime = TimeSpan.FromSeconds(tokenLifetimeNumber);

        Audiences = section.GetOrThrow<string[]>("Audiences");
        if (Audiences.Length == 0) throw new Exception(@"""Audiences"" array must contains more than 0 elements!");

        var refreshTokenLifetimeNumber = section.GetOrThrow<ulong>("RefreshLifetime");
        RefreshTokenLifetime = TimeSpan.FromSeconds(refreshTokenLifetimeNumber);
    }

    public SymmetricSecurityKey GetSymmetricSecurityKey() => new(_secret);

    public string GenerateJwtToken(ulong userId, string role, string? audience = null)
    {
        var jwt = new JwtSecurityToken(
            issuer: Issuer,
            expires: DateTime.UtcNow.Add(TokenLifetime),
            claims: [new Claim("userId", userId.ToString()), new Claim(ClaimTypes.Role, role)],
            audience: audience ?? Audiences.First(),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public bool IsJwtTokenSignatureValid(string jwtToken)
    {
        try
        {
            var parameters = ValidationParameters;
            parameters.LifetimeValidator = null;
            parameters.ValidateLifetime = false;
            new JwtSecurityTokenHandler().ValidateToken(jwtToken, parameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}