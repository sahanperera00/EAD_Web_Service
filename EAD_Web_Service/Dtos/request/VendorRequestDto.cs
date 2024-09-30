namespace EAD_Web_Service.Dtos.request;

public class VendorRequestDto
{
    private object value;

    public VendorRequestDto(object value)
    {
        this.value = value;
    }

    public string Name { get; set; }
}
