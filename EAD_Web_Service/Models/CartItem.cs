using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class CartItem
{
    [BsonElement("productId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; }
    [BsonElement("name")]
    public string Name { get; set; }
    [BsonElement("quantity")]
    public int Quantity { get; set; }
    [BsonElement("inventoryCount")]
    public int InventoryCount { get; set; }
    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }
    [BsonElement("images")]
    public List<string> Images { get; set; }
}
