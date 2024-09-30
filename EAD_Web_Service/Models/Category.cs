using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("name")]
    public required string Name { get; set; }
    [BsonElement("isActive")]
    public bool? IsActive { get; set; }
    [BsonElement("createdAt")]
    public DateTime? CreatedAt { get; set; }
    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}
