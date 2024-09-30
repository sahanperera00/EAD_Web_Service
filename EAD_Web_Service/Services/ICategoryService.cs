using EAD_Web_Service.Dtos;

namespace EAD_Web_Service.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto> GetCategoryByIdAsync(string id);
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    Task<CategoryDto?> UpdateCategoryAsync(string id, CategoryDto categoryDto);
    Task DeleteCategoryAsync(string id);
    Task<CategoryDto> ActivateCategoryAsync(string id);
    Task<CategoryDto> DeactivateCategoryAsync(string id);
}
