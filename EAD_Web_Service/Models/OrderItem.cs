using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using EAD_Web_Service.Enums;

namespace EAD_Web_Service.Models;

public class OrderItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("productId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; }
    [BsonElement("quantity")]
    public int Quantity { get; set; }
    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public ItemStatus? Status { get; set; } = ItemStatus.Processing;
}
