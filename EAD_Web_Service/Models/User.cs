using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    [BsonElement("email")]
    [BsonRequired]
    public required string Email { get; set; }
    [BsonElement("username")]
    [BsonRequired]
    public required string Username { get; set; }
    [BsonElement("password")]
    [BsonRequired]
    public required string Password { get; set; }
    [BsonElement("role")]
    [BsonRequired]
    public required string Role { get; set; }
    [BsonElement("isPending")]
    public required bool IsPending { get; set; }
    [BsonElement("isActive")]
    public required bool IsActive { get; set; }
    [BsonElement("createdAt")]
    public required DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")]
    public required DateTime UpdatedAt { get; set; }
}
