using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class CartItem
{
    [BsonElement("productId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; }
    [BsonElement("quantity")]
    public int Quantity { get; set; }
    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }
}
