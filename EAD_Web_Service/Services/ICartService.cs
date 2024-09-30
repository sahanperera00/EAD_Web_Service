using EAD_Web_Service.Dtos;

namespace EAD_Web_Service.Services;

public interface ICartService
{
    Task<CartDto> GetCartByUserIdAsync(string userId);
    Task<CartDto> CreateCartAsync(string userId);
    Task<CartDto?> AddToCartAsync(string userId, string productId);
    Task<CartDto?> RemoveFromCartAsync(string userId, string productId);
    Task<bool> DeleteCartAsync(string cartId);
}
