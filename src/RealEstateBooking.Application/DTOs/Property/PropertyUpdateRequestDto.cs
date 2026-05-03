using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Application.DTOs.Property;

public class PropertyUpdateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public int NumberOfBedrooms { get; set; }
    public int NumberOfBathrooms { get; set; }
    public decimal SizeInSquareMeters { get; set; }
    public PropertyType Type { get; set; }
}
