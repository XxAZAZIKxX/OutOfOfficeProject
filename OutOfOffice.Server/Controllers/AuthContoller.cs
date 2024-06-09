using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Core.Requests;
using OutOfOffice.Server.Config;
using OutOfOffice.Server.Services;

namespace OutOfOffice.Server.Controllers;

[ApiController, Route("[controller]")]
public class AuthController(IAuthRepository authRepository, JwtConfiguration jwtConfiguration) : ControllerBase
{
    [HttpPost, Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        return await Task.Run(new Func<IActionResult>(() =>
        {
            var user = authRepository.IsUserCredentialsLegit(authRequest);
            if (user is null) return Unauthorized("User credentials are incorrect");
            return Ok(new
            {
                SecurityToken = jwtConfiguration.GenerateJwtToken(user.Id, user.Position.ToString("G")),
                Role = user.Position.ToString("G")
            });
        }));
    }
}