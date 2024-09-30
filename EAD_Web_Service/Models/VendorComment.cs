using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service.Models;

public class VendorComment
{
    [BsonElement("customerId")]
    public required ObjectId CustomerId { get; set; }
    [BsonElement("comment")]
    public required string Comment { get; set; }
    [BsonElement("createdAt")]
    public required DateTime CreatedAt { get; set; }
}