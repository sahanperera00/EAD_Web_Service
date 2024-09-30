using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;

namespace EAD_Web_Service.Services;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllProductsAsync();
    Task<ProductResponseDto> GetProductByIdAsync(string productId);
    Task<ProductResponseDto?> CreateProductAsync(ProductRequestDto productRequestDto, string userId);
    Task<ProductResponseDto?> UpdateProductAsync(string productId, ProductRequestDto productRequestDto);
    Task<bool> DeleteProductAsync(string productId, string userId);
    Task<ProductResponseDto?> GetProductByProductIdAndVendorIdAsync(string productId, string vendorId);
    Task<List<ProductResponseDto>> GetProductsByVendorIdAsync(string vendorId);
    Task<List<ProductResponseDto>> GetProductsByCategoryIdAsync(string categoryId);
}
