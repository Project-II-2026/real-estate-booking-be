using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Domain.Entities;

public class Property : Audit
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public List<PropertyImage> Images { get; set; } = [];
    public int NumberOfBedrooms { get; set; }
    public int NumberOfBathrooms { get; set; }
    public decimal SizeInSquareMeters { get; set; }
    public PropertyType Type { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
