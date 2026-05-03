namespace RealEstateBooking.Application.DTOs.PropertyImage;

public class PropertyImageStatusResponseDto
{
    public int ImageId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
