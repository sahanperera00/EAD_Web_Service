using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service.Models;

public class VendorComment
{
    [BsonElement("customerId")]
    public string CustomerId { get; set; }
    [BsonElement("comment")]
    public string Comment { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
}