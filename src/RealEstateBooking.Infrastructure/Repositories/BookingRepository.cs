using Microsoft.EntityFrameworkCore;

using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Domain.Enums;
using RealEstateBooking.Infrastructure.Persistance;

namespace RealEstateBooking.Infrastructure.Repositories;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    public async Task AddAsync(Booking booking)
    {
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        context.Bookings.Update(booking);
        await context.SaveChangesAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id) =>
        await context.Bookings
            .Include(b => b.Property)
            .Include(b => b.Visitor)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<bool> HasOverlapAsync(int propertyId, DateTime startUtc, DateTime endUtc) =>
        await context.Bookings
            .AnyAsync(b =>
                b.PropertyId == propertyId
                && b.Status == BookingStatus.Confirmed
                && b.StartTime < endUtc
                && b.EndTime > startUtc);

    public async Task<bool> HasCompletedBookingAsync(int propertyId, int visitorId) =>
        await context.Bookings
            .AnyAsync(b =>
                b.PropertyId == propertyId
                && b.VisitorId == visitorId
                && b.Status == BookingStatus.Confirmed
                && b.EndTime <= DateTime.UtcNow);

    public async Task<IEnumerable<Booking>> GetTakenSlotsAsync(int propertyId, DateTime fromUtc) =>
        await context.Bookings
            .Where(b => b.PropertyId == propertyId
                        && b.Status == BookingStatus.Confirmed
                        && b.EndTime > fromUtc)
            .OrderBy(b => b.StartTime)
            .ToListAsync();

    public async Task<(IEnumerable<Booking> Items, int TotalCount)> GetPagedForVisitorAsync(
        int visitorId, int page, int pageSize, BookingFilterDto? filter = null)
    {
        var query = context.Bookings
            .Include(b => b.Property)
            .Include(b => b.Visitor)
            .Where(b => b.VisitorId == visitorId)
            .Where(b => filter == null || filter.Status == null || b.Status == filter.Status)
            .Where(b => filter == null || filter.From == null || b.StartTime >= filter.From)
            .Where(b => filter == null || filter.To == null || b.StartTime <= filter.To)
            .OrderByDescending(b => b.StartTime);

        int totalCount = await query.CountAsync();
        List<Booking> items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<(IEnumerable<Booking> Items, int TotalCount)> GetPagedForPropertyAsync(
        int propertyId, int page, int pageSize, BookingFilterDto? filter = null)
    {
        var query = context.Bookings
            .Include(b => b.Property)
            .Include(b => b.Visitor)
            .Where(b => b.PropertyId == propertyId)
            .Where(b => filter == null || filter.Status == null || b.Status == filter.Status)
            .Where(b => filter == null || filter.From == null || b.StartTime >= filter.From)
            .Where(b => filter == null || filter.To == null || b.StartTime <= filter.To)
            .OrderByDescending(b => b.StartTime);

        int totalCount = await query.CountAsync();
        List<Booking> items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }
}
