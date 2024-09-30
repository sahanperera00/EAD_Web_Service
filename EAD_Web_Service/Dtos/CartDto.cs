using EAD_Web_Service.Models;

namespace EAD_Web_Service.Dtos;

public class CartDto
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public List<CartItemDto>? Items { get; set; }
    public decimal? TotalPrice { get; set; }
}
