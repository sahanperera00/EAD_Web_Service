using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class VendorRatings
{
    [BsonElement("average")]
    public required double Average { get; set; }
    [BsonElement("total_reviews")]
    public required int TotalReviews { get; set; }
}