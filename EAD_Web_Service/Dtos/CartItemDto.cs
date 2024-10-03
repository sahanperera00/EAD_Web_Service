namespace EAD_Web_Service.Dtos;

public class CartItemDto
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int InventoryCount { get; set; }
    public decimal Price { get; set; }
    public List<string>? Images { get; set; }
}
