using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Application.DTOs.Booking;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public int VisitorId { get; set; }
    public string VisitorUsername { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
}
