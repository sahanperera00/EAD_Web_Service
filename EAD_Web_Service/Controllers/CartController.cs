using EAD_Web_Service.Dtos;
using EAD_Web_Service.Services;
using EAD_Web_Service.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EAD_Web_Service.Controllers;

[Route("api/cart")]
[ApiController]
public class CartController(ICartService _cartService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<CartDto>> GetCartByUserId()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            return BadRequest("Invalid user or permissions.");
        }

        var response = await _cartService.GetCartByUserIdAsync(currentUserId);

        if (response == null)
        {
            return NotFound("Invalid cart");
        }
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateCart()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            return BadRequest("Invalid user or permissions.");
        }

        var response = await _cartService.CreateCartAsync(currentUserId);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create the cart");
        }
        return Ok(response);
    }

    [HttpPost("addItem/{productId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddToCart(string productId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            return BadRequest("Invalid user or permissions.");
        }

        var updatedCart = await _cartService.AddToCartAsync(currentUserId, productId);

        if (updatedCart == null)
        {
            return NotFound("Failed to add the item to the cart.");
        }
        return Ok(updatedCart);
    }

    [HttpPost("removeItem/{productId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RemoveFromCart(string productId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            return BadRequest("Invalid user or permissions.");
        }
        var updatedCart = await _cartService.RemoveFromCartAsync(currentUserId, productId);

        if (updatedCart == null)
        {
            return NotFound("Product not found in the cart.");
        }
        return Ok(updatedCart);
    }
}
