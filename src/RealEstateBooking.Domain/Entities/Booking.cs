using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Domain.Entities;

public class Booking : Audit
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public int VisitorId { get; set; }
    public User Visitor { get; set; } = null!;
}
