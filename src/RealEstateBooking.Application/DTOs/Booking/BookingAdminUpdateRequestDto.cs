using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Application.DTOs.Booking;

public class BookingAdminUpdateRequestDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
}
