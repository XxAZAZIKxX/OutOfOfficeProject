using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Requests;
using OutOfOffice.Core.Responses;
using OutOfOffice.Server.Config;
using OutOfOffice.Server.Core.Extensions;
using OutOfOffice.Server.Repositories;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("[controller]")]
public class AuthController(
    IAuthRepository authRepository,
    JwtConfiguration jwtConfiguration,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<AuthController> logger
    ) : ControllerBase
{
    [HttpPost, Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        var user = await authRepository.IsUserCredentialsLegitAsync(authRequest.Username, authRequest.PasswordHash);
        if (user is null) return Unauthorized("User credentials are incorrect");
        var refreshToken = GenerateRefreshToken();
        await refreshTokenRepository.SetRefreshTokenAsync(user.Id, refreshToken, jwtConfiguration.RefreshTokenLifetime);
        logger.LogInformation("Employee with {id} id succesfully logged in", user.Id);
        return Ok(GenerateTokenResponse(user.Id, user.Position.ToString("G"), refreshToken));
    }

    [HttpPost, Route("refresh")]
    public async Task<IActionResult> RefreshToken([FromHeader] string authorization, [FromHeader] string refreshToken)
    {
        var jwtTokenString = authorization.Replace("Bearer ", "");
        var token = new JwtSecurityToken(jwtTokenString);

        if (jwtConfiguration.IsJwtTokenSignatureValid(jwtTokenString) is false)
        {
            return Unauthorized("Invalid authorization token!");
        }

        var userId = token.Claims.GetUserId();
        var role = token.Claims.GetUserRole();

        var isRefreshValid = await refreshTokenRepository.IsRefreshTokenValidAsync(userId, refreshToken);
        if (isRefreshValid is false) return Unauthorized("Refresh token is invalid or expired!");

        var generatedRefreshToken = GenerateRefreshToken();
        await refreshTokenRepository.SetRefreshTokenAsync(userId, generatedRefreshToken, jwtConfiguration.RefreshTokenLifetime);

        logger.LogInformation("Employee with {id} id succesfully refreshed token", userId);

        return Ok(GenerateTokenResponse(userId, role, generatedRefreshToken));
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

    private static string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}