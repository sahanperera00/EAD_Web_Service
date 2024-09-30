using EAD_Web_Service.Models;

namespace EAD_Web_Service.Dtos.response;

public class VendorResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Owner { get; set; }
    public List<string> Products { get; set; }
    public VendorRatings Ratings { get; set; }
    public List<VendorComment> Comments { get; set; }
}
