using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class Cart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string UserId { get; set; }
    [BsonElement("items")]
    public List<CartItem>? Items { get; set; }
    [BsonElement("totalPrice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? TotalPrice { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
