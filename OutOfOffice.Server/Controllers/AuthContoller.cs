using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Requests;
using OutOfOffice.Core.Responses;
using OutOfOffice.Server.Config;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Services;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("[controller]")]
public class AuthController(
    IAuthRepository authRepository,
    JwtConfiguration jwtConfiguration,
    IRefreshTokenRepository refreshTokenRepository
    ) : ControllerBase
{
    [HttpPost, Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        return await Task.Run(new Func<IActionResult>(() =>
        {
            var user = authRepository.IsUserCredentialsLegit(authRequest);
            if (user is null) return Unauthorized("User credentials are incorrect");
            var refreshToken = GenerateRefreshToken();
            refreshTokenRepository.SetRefreshToken(user.Id, refreshToken, jwtConfiguration.RefreshTokenLifetime);
            return Ok(GenerateTokenResponse(user.Id, user.Position.ToString("G"), refreshToken));
        }));
    }

    [HttpPost, Route("refresh")]
    public async Task<IActionResult> RefreshToken([FromHeader] string authorization, [FromHeader] string refreshToken)
    {
        return await Task.Run(new Func<IActionResult>(() =>
        {
            var token = new JwtSecurityToken(authorization.Replace("Bearer ", ""));

            var userId = token.Claims.GetUserId();
            var role = token.Claims.GetUserRole();
            if (refreshTokenRepository.IsRefreshTokenValid(userId, refreshToken) is false)
                return Unauthorized("Refresh token is invalid or expired!");
            
            var generatedRefreshToken = GenerateRefreshToken();
            refreshTokenRepository.SetRefreshToken(userId, generatedRefreshToken, jwtConfiguration.RefreshTokenLifetime);
            return Ok(GenerateTokenResponse(userId, role, generatedRefreshToken));
        }));
    }

    private TokenResponse GenerateTokenResponse(ulong userId, string role, string refreshToken)
    {
        return new TokenResponse
        {
            BearerToken = jwtConfiguration.GenerateJwtToken(userId, role),
            UserId = userId,
            Role = role,
            ExpireAt = DateTimeOffset.UtcNow.Add(jwtConfiguration.TokenLifetime).ToUnixTimeSeconds(),
            RefreshToken = refreshToken
        };

    }

    private string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}