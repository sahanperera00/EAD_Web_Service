using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EAD_Web_Service.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserService _userService, IVendorService _vendorService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,CSR")]
    public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers() =>
        await _userService.GetAllUsersAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(string id)
    {
        var response = await _userService.GetUserByIdAsync(id);

        if (response == null)
        {
            return NotFound("User not found");
        }
        return response;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(UserRequestDto userDto)
    {
        if (userDto == null ||
            string.IsNullOrWhiteSpace(userDto.Email) ||
            string.IsNullOrWhiteSpace(userDto.Username) ||
            string.IsNullOrWhiteSpace(userDto.Password) ||
            string.IsNullOrWhiteSpace(userDto.Role))
        {
            return BadRequest("Missing required fields.");
        }

        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userDto.Role == "Vendor" && currentUserRole != "Admin")
        {
            return StatusCode(StatusCodes.Status403Forbidden, "Invalid role or permissions");
        }
        var response = await _userService.CreateUserAsync(userDto);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create the user");
        }
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserRequestDto userRequestDto)
    {
        if (userRequestDto == null)
        {
            return BadRequest("Missing required fields");
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var user = await _userService.GetUserByIdAsync(currentUserId);
        var vendor = await _vendorService.GetVendorByUserIdAsync(currentUserId);

        if (user == null)
        {
            return BadRequest("Invalid user or permissions.");
        }

        var response = await _userService.UpdateUserAsync(currentUserId, userRequestDto);

        if (currentUserRole == "Vendor")
        {
            _vendorService.UpdateVendorAsync(vendor.Id, userRequestDto.Username);
        }

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the user");
        }
        return Ok(response);
    }

    [HttpPost("activate/{id}")]
    [Authorize(Roles = "Admin,CSR")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound("Invalid user");
        }

        var response = await _userService.ActivateUserAsync(id);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to activate the user");
        }
        return Ok("User activated successfully");
    }

    [HttpPost("deactivate/{id}")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound("Invalid user");
        }
        
        var response = await _userService.DeactivateUserAsync(id);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to deactivate the user");
        }
        return Ok("User deactivated successfully");
    }

}
