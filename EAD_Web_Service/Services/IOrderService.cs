using EAD_Web_Service.Dtos;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Web_Service.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<List<OrderDto>> GetOrdersByUserIdAsync(string userId);
    Task<OrderDto?> GetOrderByIdAsync(string orderId);
    Task<OrderDto> CreateOrderAsync(string userId, OrderDto orderDtod);
    Task<OrderDto?> UpdateOrderAsync(string orderId, OrderDto OrderDto);
    Task<bool> CancelOrderAsync(string orderId);
    Task<bool> RequestOrderCancellationAsync(CancelOrderRequestDto cancelOrderRequestDto);
    Task<List<OrderItemDto>> GetOrderItemsByVendorIdAsync(string vendorId);
    Task<bool> UpdateOrderItemStatusAsync(string orderItemId, ItemStatus newStatus);
    Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);
}
