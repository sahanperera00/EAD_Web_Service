namespace EAD_Web_Service.Dtos.request;

public class VendorRatingRequestDto
{
    public required string CustomerId { get; set; }
    public double Rating { get; set; }
    public required string Comment { get; set; }
}
