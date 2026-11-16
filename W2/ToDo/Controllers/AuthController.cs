using Microsoft.AspNetCore.Mvc;
using ToDo.DTOs;
using ToDo.Services;

namespace ToDo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request);

        if (!result.Succeeded)
            return BadRequest(new { message = result.Message, errors = result.Errors });

        return Ok(new
        {
            message = result.Message,
            email = request.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);

        if (!result.Succeeded)
            return Unauthorized(new { message = result.Message });

        return Ok(new
        {
            message = result.Message,
            email = request.Email,
            token = result.Token,
            expiration = result.Expiration
        });
    }
}
