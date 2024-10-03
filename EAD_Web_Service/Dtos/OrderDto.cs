namespace EAD_Web_Service.Dtos;

public class OrderDto
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? CartId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DeliveryAddress { get; set; }
    public List<OrderItemDto>? Items { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Status { get; set; }
    public bool? IsCancelRequested { get; set; }
    public string? Note { get; set; }
}
