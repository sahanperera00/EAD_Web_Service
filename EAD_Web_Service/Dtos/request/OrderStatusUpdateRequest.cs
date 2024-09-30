using EAD_Web_Service.Enums;

namespace EAD_Web_Service.Dtos.request;

public class OrderStatusUpdateRequest
{
    public required string Id { get; set; }
    public required OrderStatus NewStatus { get; set; }
}
