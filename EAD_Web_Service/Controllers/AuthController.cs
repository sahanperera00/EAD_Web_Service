using Microsoft.AspNetCore.Mvc;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Services;
using EAD_Web_Service.Services.Impl;

namespace EAD_Web_Service.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService _userService, TokenService tokenService, ICartService _cartService) : ControllerBase
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
        var oldCart = await _cartService.GetCartByUserIdAsync(user.Id);

        if (oldCart == null && user.Role.Equals("Customer"))
        {
            var newCart = await _cartService.CreateCartAsync(user.Id);

            if (newCart == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create the cart");
            }
        }
        return Ok(new { Token = token });
    }

}
