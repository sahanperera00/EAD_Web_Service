using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("vendorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string VendorId { get; set; }
    [BsonElement("name")]
    public required string Name { get; set; }
    [BsonElement("description")]
    public required string Description { get; set; }
    [BsonElement("categoryId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string CategoryId { get; set; }
    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public required decimal Price { get; set; }
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = false;
    [BsonElement("inventoryCount")]
    public int? InventoryCount { get; set; }
    [BsonElement("lowStockAlert")]
    public int? LowStockAlert { get; set; }
    [BsonElement("images")]
    public string[]? Images { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
