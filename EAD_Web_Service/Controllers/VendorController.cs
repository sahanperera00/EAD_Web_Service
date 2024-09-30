using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Models;
using EAD_Web_Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Web_Service.Controllers;

[Route("api/vendor")]
[ApiController]
public class VendorController(IVendorService _vendorService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<VendorResponseDto>>> GetAllVendors() =>
        await _vendorService.GetAllVendorsAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<VendorResponseDto>> GetVendorById(string id)
    {
        var response = await _vendorService.GetVendorByIdAsync(id);

        if (response == null)
        {
            return NotFound("Invalid category");
        }
        return response;
    }

    [HttpPost("{vendorId}/rating")]
    public async Task<IActionResult> AddVendorRating(string vendorId, [FromBody] VendorRatingRequestDto request)
    {
        var result = await _vendorService.AddVendorRatingAsync(vendorId, request.CustomerId, request.Rating, request.Comment);

        if (!result)
        {
            return BadRequest(new { Message = "Rating submission failed or already rated." });
        }

        return Ok(new { Message = "Rating and comment added successfully." });
    }

    [HttpPut("{vendorId}/comment")]
    public async Task<IActionResult> UpdateVendorComment(string vendorId, [FromBody] VendorRatingRequestDto request)
    {
        var result = await _vendorService.UpdateVendorCommentAsync(vendorId, request.CustomerId, request.Comment);

        if (!result)
        {
            return BadRequest(new { Message = "Comment update failed." });
        }

        return Ok(new { Message = "Comment updated successfully." });
    }

}
