using EAD_Web_Service.Enums;

namespace EAD_Web_Service.Dtos.request;

public class OrderItemStatusUpdateRequest
{
    public required string OrderItemId { get; set; }
    public required ItemStatus NewStatus { get; set; }
}
