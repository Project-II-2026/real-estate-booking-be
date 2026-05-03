namespace RealEstateBooking.Application.DTOs.PropertyImage;

public class PropertyImagePresignedUrlResponseDto
{
    public int ImageId { get; set; }
    public string PresignedUrl { get; set; } = string.Empty;
    public string S3Key { get; set; } = string.Empty;
    public Dictionary<string, string> RequiredHeaders { get; set; } = [];
}
