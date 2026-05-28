using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Application.DTOs.Common;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(BookingCreateRequestDto request, int visitorId);
    Task<BookingResponseDto> GetByIdAsync(int id, int requestingUserId, bool isAdmin = false);
    Task<PaginationResponseDto<BookingResponseDto>> GetMineAsync(
        int visitorId, PaginationRequestDto parameters, BookingFilterDto? filter = null);
    Task<PaginationResponseDto<BookingResponseDto>> GetForPropertyAsync(
        int propertyId, int requestingUserId, PaginationRequestDto parameters, BookingFilterDto? filter = null, bool isAdmin = false);
    Task<IEnumerable<BookingSlotDto>> GetAvailabilityAsync(int propertyId);
    Task CancelAsync(int id, int requestingUserId, bool isAdmin = false);
    Task<BookingResponseDto> AdminUpdateAsync(int id, BookingAdminUpdateRequestDto request);
    Task AdminDeleteAsync(int id);
    Task<PaginationResponseDto<BookingResponseDto>> AdminGetAllAsync(
        PaginationRequestDto parameters, BookingFilterDto? filter = null);
}
