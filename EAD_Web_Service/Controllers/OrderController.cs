using EAD_Web_Service.Dtos;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EAD_Web_Service.Controllers;

[Route("api/order")]
[ApiController]
public class OrderController(IOrderService _orderService, ICartService _cartService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,CSR")]
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders() =>
        await _orderService.GetAllOrdersAsync();

    [HttpGet("userOrders")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetOrdersByUserId()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            return BadRequest("Invalid user or permissions.");
        }

        var orders = await _orderService.GetOrdersByUserIdAsync(currentUserId);
        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    [Authorize(Roles = "Admin,CSR,Customer")]
    public async Task<IActionResult> GetOrderById(string orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);

        if (order == null) return NotFound("Order not found.");

        return Ok(order);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateOrder(OrderRequestDto orderRequestDto)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            return BadRequest("Invalid user or permissions.");
        }

        var cart = await _cartService.GetCartByUserIdAsync(currentUserId);
        var orderResponse = await _orderService.CreateOrderAsync(currentUserId, orderRequestDto, cart);

        if (orderResponse == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create the order.");
        }
        await _cartService.DeleteCartAsync(cart.Id);
        await _cartService.CreateCartAsync(currentUserId);
        return Ok(orderResponse);
    }

    [HttpPut("update/{orderId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateOrder(string orderId, [FromBody] OrderDto orderDto)
    {
        var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderDto);

        if (updatedOrder == null)
        {
            return BadRequest("Order cannot be updated (either it's dispatched or doesn't exist).");
        }

        return Ok(updatedOrder);
    }

    [HttpDelete("cancel/{orderId}")]
    [Authorize(Roles = "Admin,CSR")]
    public async Task<IActionResult> CancelOrder(string orderId)
    {
        var success = await _orderService.CancelOrderAsync(orderId);

        if (!success) return BadRequest("Order cannot be canceled.");

        return Ok("Order canceled successfully.");
    }

    [HttpPost("cancelOrderRequest")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequestDto request)
    {
        var isCancellationRequested = await _orderService.RequestOrderCancellationAsync(request);

        if (!isCancellationRequested)
        {
            return BadRequest(new { Message = "Order not found or cannot be cancelled." });
        }
        return Ok(new { Message = "Order cancellation request has been submitted." });
    }


    [HttpGet("orderItems/{vendorId}")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> GetOrderItemsByVendorId(string vendorId)
    {
        if (string.IsNullOrEmpty(vendorId))
        {
            return BadRequest("Vendor ID cannot be null or empty.");
        }

        var pendingOrderItems = await _orderService.GetOrderItemsByVendorIdAsync(vendorId);

        if (pendingOrderItems == null || !pendingOrderItems.Any())
        {
            return NotFound("No pending order items found for this vendor.");
        }
        return Ok(pendingOrderItems);
    }

    [HttpPut("orderItem/status")]
    [Authorize(Roles = "Admin,CSR,Vendor")]
    public async Task<IActionResult> UpdateOrderItemStatus([FromBody] OrderItemStatusUpdateRequest orderItemStatusUpdateRequest)
    {
        if (orderItemStatusUpdateRequest == null)
        {
            return BadRequest("Missing required fields.");
        }

        var result = await _orderService.UpdateOrderItemStatusAsync(orderItemStatusUpdateRequest.OrderItemId, orderItemStatusUpdateRequest.NewStatus);

        if (!result)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the order item status");
        }
        return Ok("Order item status updated successfully.");
    }

    [HttpPut("updateStatus")]
    [Authorize(Roles = "Admin,CSR")]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderStatusUpdateRequest orderStatusUpdateRequest)
    {
        var isUpdated = await _orderService.UpdateOrderStatusAsync(orderStatusUpdateRequest.Id, orderStatusUpdateRequest.NewStatus);

        if (!isUpdated)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the order status");
        }
        return Ok("Order status updated successfully.");
    }

}
