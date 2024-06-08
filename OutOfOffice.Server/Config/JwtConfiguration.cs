using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OutOfOffice.Server.Config;

public class JwtConfiguration : BaseConfiguration
{
    public readonly string Issuer;
    public readonly TimeSpan TokenDuration;
    public readonly string[] Audiences;

    private readonly byte[] _secret;

    public JwtConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("JwtConfiguration");
        Issuer = section.GetRequiredSection("Issuer").Get<string?>() ?? throw GetSimpleMissingException("Issuer");

        var secretString = section.GetRequiredSection("Secret").Get<string?>() ?? throw GetSimpleMissingException("Secret");
        _secret = Encoding.UTF8.GetBytes(secretString);

        var durationNumber = section.GetRequiredSection("Duration").Get<ulong?>() ?? throw GetSimpleMissingException("Duration");
        TokenDuration = TimeSpan.FromSeconds(durationNumber);

        Audiences = section.GetRequiredSection("Audiences").Get<string[]?>() ?? throw GetSimpleMissingException("Audiences");
        if (Audiences.Length == 0) throw new Exception(@"""Audiences"" array must contains more than 0 elements!");
    }

    public SymmetricSecurityKey GetSymmetricSecurityKey() => new(_secret);

    public string GenerateJwtToken(ulong userId, string role, string? audience = null)
    {
        var jwt = new JwtSecurityToken(
            issuer: Issuer,
            expires: DateTime.UtcNow.Add(TokenDuration),
            claims: [new Claim("id", userId.ToString()), new Claim(ClaimTypes.Role, role)],
            audience: audience ?? Audiences.First(),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}