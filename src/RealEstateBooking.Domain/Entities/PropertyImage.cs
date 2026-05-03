using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Domain.Entities;

public class PropertyImage : Audit
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public string S3Key { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public PropertyImageStatus Status { get; set; }
}
