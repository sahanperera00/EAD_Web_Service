using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EAD_Web_Service.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController(IProductService _productService, IVendorService _vendorService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAllProducts() =>
        await _productService.GetAllProductsAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(string id)
    {
        var response = await _productService.GetProductByIdAsync(id);

        if (response == null)
        {
            return NotFound("Invalid product");
        }
        return response;
    }

    [HttpPost]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> CreateProduct(ProductRequestDto productRequestDto)
    {
        if (productRequestDto == null)
        {
            return BadRequest("Missing required fields");
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
        {
            return BadRequest("Invalid user or permissions.");
        }

        var response = await _productService.CreateProductAsync(productRequestDto, currentUserId);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create the Product");
        }
        return Ok("Product created successfully");
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UpdateProduct(string id, ProductRequestDto productRequestDto)
    {
        if (productRequestDto == null)
        {
            return BadRequest("Missing required fields");
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var vendor = _vendorService.GetVendorByUserIdAsync(currentUserId).Result;
        var product = await _productService.GetProductByProductIdAndVendorIdAsync(id, vendor.Id);

        if (product == null)
        {
            return NotFound("Invalid product or permissions");
        }

        var response = await _productService.UpdateProductAsync(id, productRequestDto);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the product");
        }
        return Ok("Product updated successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var vendor = _vendorService.GetVendorByUserIdAsync(currentUserId).Result;
        var product = await _productService.GetProductByProductIdAndVendorIdAsync(id, vendor.Id);

        if (product == null)
        {
            return BadRequest("Invalid product or permissions");
        }

        var response = await _productService.DeleteProductAsync(id, currentUserId);

        if (!response)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete the Product");
        }
        return Ok("Product deleted successfully");
    }

    [HttpGet("vendor/{id}")]
    public async Task<ActionResult<List<ProductResponseDto>>> GetProductsByVendorId(string id)
    {
        var products =  await _productService.GetProductsByVendorIdAsync(id);
        return Ok(products);
    }

    [HttpGet("category/{id}")]
    public async Task<ActionResult<List<ProductResponseDto>>> GetProductsByCategoryId(string id)
    {
        var products = await _productService.GetProductsByCategoryIdAsync(id);
        return Ok(products);
    }
}
