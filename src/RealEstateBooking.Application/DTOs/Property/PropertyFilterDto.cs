using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Application.DTOs.Property;

public class PropertyFilterDto
{
    public PropertyType? Type { get; set; }
    public string? Location { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; }
}
