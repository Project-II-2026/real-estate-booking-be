using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Application.DTOs.Common;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(BookingCreateRequestDto request, int visitorId);
    Task<BookingResponseDto> GetByIdAsync(int id, int requestingUserId);
    Task<PaginationResponseDto<BookingResponseDto>> GetMineAsync(
        int visitorId, PaginationRequestDto parameters, BookingFilterDto? filter = null);
    Task<PaginationResponseDto<BookingResponseDto>> GetForPropertyAsync(
        int propertyId, int requestingUserId, PaginationRequestDto parameters, BookingFilterDto? filter = null);
    Task<IEnumerable<BookingSlotDto>> GetAvailabilityAsync(int propertyId);
    Task CancelAsync(int id, int requestingUserId);
}
