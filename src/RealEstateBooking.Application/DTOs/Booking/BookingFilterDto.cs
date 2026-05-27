using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Application.DTOs.Booking;

public class BookingFilterDto
{
    public BookingStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
