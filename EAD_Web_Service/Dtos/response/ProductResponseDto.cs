namespace EAD_Web_Service.Dtos.response;

public class ProductResponseDto
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CategoryId { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = false;
    public int InventoryCount { get; set; }
    public int LowStockAlert { get; set; }
    public List<string>? Images { get; set; }
}
