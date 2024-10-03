using EAD_Web_Service.Models;

namespace EAD_Web_Service.Dtos.response;

public class ProductVendorResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public VendorRatings Ratings { get; set; }
    public List<VendorComment> Comments { get; set; }
}
