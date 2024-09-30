using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Models;

namespace EAD_Web_Service.Services;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> GetUserByIdAsync(string id);
    Task<UserResponseDto?> CreateUserAsync(UserRequestDto userDto);
    Task<UserResponseDto?> UpdateUserAsync(string id, UserRequestDto userRequestDto);
    Task<UserResponseDto> ActivateUserAsync(string id);
    Task<UserResponseDto> DeactivateUserAsync(string id);
    Task<User?> GetUserByEmailAsync(string email);
}
