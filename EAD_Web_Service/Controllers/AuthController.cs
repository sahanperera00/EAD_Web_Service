using Microsoft.AspNetCore.Mvc;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Services;
using EAD_Web_Service.Services.Impl;
using System.Security.Claims;

namespace EAD_Web_Service.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService _userService, TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
    {
        if (loginRequestDto == null ||
            string.IsNullOrEmpty(loginRequestDto.Email) ||
            string.IsNullOrEmpty(loginRequestDto.Password))
        {
            return BadRequest("Missing required fields.");
        }

        var user = await _userService.GetUserByEmailAsync(loginRequestDto.Email);

        if (user == null ||
            string.IsNullOrEmpty(user.Email) || 
            string.IsNullOrEmpty(user.Password) ||
            !BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        if ((bool)!user.IsActive) 
        {
            return Unauthorized("Inactive Account!");
        }

        var token = tokenService.GenerateToken(user.Id ,user.Email, user.Role);
        return Ok(new { Token = token });
    }

}
