namespace RealEstateBooking.Application.Options;

public class AwsSettings
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ServiceUrl { get; set; }
}
