using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using EAD_Web_Service.Enums;

namespace EAD_Web_Service.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string UserId { get; set; }

    [BsonElement("cartId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string CartId { get; set; }
    [BsonElement("phoneNumber")]
    public required string PhoneNumber { get; set; }
    [BsonElement("deliveryAddress")]
    public required string DeliveryAddress { get; set; }

    [BsonElement("items")]
    public List<OrderItem>? Items { get; set; }

    [BsonElement("totalPrice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalPrice { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public OrderStatus Status { get; set; } = OrderStatus.Processing;
    [BsonElement("isCancelRequested")]
    public bool IsCancelRequested { get; set; } = false;
    [BsonElement("note")]
    public string Note { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
