namespace EAD_Web_Service.Dtos.response;

public class UserResponseDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsPending { get; set; }
}
