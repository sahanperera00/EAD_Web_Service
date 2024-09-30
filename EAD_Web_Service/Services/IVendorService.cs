using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;

namespace EAD_Web_Service.Services;

public interface IVendorService
{
    Task<List<VendorResponseDto>> GetAllVendorsAsync();
    Task<VendorResponseDto> GetVendorByIdAsync(string id);
    Task<VendorResponseDto> CreateVendorAsync(string userId, string userName);
    Task<VendorResponseDto?> UpdateVendorAsync(string id, string name);
    Task<VendorResponseDto> GetVendorByUserIdAsync(string userId);
    Task<bool> AddVendorRatingAsync(string vendorId, string customerId, double rating, string comment);
    Task<bool> UpdateVendorCommentAsync(string vendorId, string customerId, string updatedComment);

}
