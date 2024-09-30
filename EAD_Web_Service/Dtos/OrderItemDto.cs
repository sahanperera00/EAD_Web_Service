namespace EAD_Web_Service.Dtos;

public class OrderItemDto
{
    public string? Id { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
}
