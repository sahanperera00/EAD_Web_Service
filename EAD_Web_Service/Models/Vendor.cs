using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service.Models;

public class Vendor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("name")]
    public required string Name { get; set; }
    [BsonElement("owner")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Owner { get; set; }
    [BsonElement("products")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Products { get; set; }
    [BsonElement("ratings")]
    public required VendorRatings Ratings { get; set; }
    [BsonElement("comments")]
    public required List<VendorComment> Comments { get; set; }
    [BsonElement("createdAt")]
    public required DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")]
    public required DateTime UpdatedAt { get; set; }
}
