using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Web_Service.Models;

public class VendorRatings
{
    [BsonElement("average")]
    public double Average { get; set; }
    [BsonElement("total_reviews")]
    public int TotalReviews { get; set; }
}