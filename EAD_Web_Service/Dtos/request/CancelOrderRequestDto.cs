using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service.Dtos.request;

public class CancelOrderRequestDto
{
    public required string Note { get; set; }
}
