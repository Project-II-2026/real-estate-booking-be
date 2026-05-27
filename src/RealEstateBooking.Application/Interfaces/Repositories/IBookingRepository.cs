using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task<Booking?> GetByIdAsync(int id);
    Task<bool> HasOverlapAsync(int propertyId, DateTime startUtc, DateTime endUtc);
    Task<bool> HasCompletedBookingAsync(int propertyId, int visitorId);
    Task<IEnumerable<Booking>> GetTakenSlotsAsync(int propertyId, DateTime fromUtc);
    Task<(IEnumerable<Booking> Items, int TotalCount)> GetPagedForVisitorAsync(
        int visitorId, int page, int pageSize, BookingFilterDto? filter = null);
    Task<(IEnumerable<Booking> Items, int TotalCount)> GetPagedForPropertyAsync(
        int propertyId, int page, int pageSize, BookingFilterDto? filter = null);
}
