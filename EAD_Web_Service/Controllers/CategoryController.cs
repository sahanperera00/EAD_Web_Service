using Microsoft.AspNetCore.Mvc;
using EAD_Web_Service.Services;
using Microsoft.AspNetCore.Authorization;
using EAD_Web_Service.Dtos;

namespace EAD_Web_Service.Controllers;

[Route("api/category")]
[ApiController]
public class CategoryController(ICategoryService _categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAllCategories() =>
        await _categoryService.GetAllCategoriesAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(string id)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id);

        if (response == null)
        {
            return NotFound("Invalid category");
        }
        return response;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CSR,Vendor")]
    public async Task<IActionResult> CreateCategory(CategoryDto categoryDto)
    {
        if (categoryDto == null ||
            string.IsNullOrWhiteSpace(categoryDto.Name))
        {
            return BadRequest("Missing required fields");
        }
        
        var response = await _categoryService.CreateCategoryAsync(categoryDto);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create the category");
        }
        return Ok("Category created successfully");
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CSR,Vendor")]
    public async Task<IActionResult> UpdateCategory(string id, CategoryDto categoryDto)
    {
        if (categoryDto == null)
        {
            return BadRequest("Missing required fields");
        }

        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound("Invalid category");
        }

        var response = await _categoryService.UpdateCategoryAsync(id, categoryDto);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the category");
        }
        return Ok("Category updated successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound("Invalid category");
        }
        await _categoryService.DeleteCategoryAsync(id);
        return Ok("Category deleted successfully");
    }

    [HttpPost("activate/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateCategory(string id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound("Invalid category");
        }

        var response = await _categoryService.ActivateCategoryAsync(id);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to activate the category");
        }
        return Ok("Category activated successfully");
    }

    [HttpPost("deactivate/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateCategory(string id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return NotFound("Invalid category");
        }

        var response = await _categoryService.DeactivateCategoryAsync(id);

        if (response == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to deactivate the category");
        }
        return Ok("Category deactivated successfully");
    }
}
