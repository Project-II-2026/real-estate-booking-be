using Microsoft.EntityFrameworkCore;

using RealEstateBooking.Application.DTOs.Property;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Infrastructure.Persistance;

namespace RealEstateBooking.Infrastructure.Repositories;

public class PropertyRepository(AppDbContext context) : IPropertyRepository
{
    public async Task AddAsync(Property property)
    {
        await context.Properties.AddAsync(property);
        await context.SaveChangesAsync();
    }

    public async Task<Property?> GetByIdAsync(int id) =>
        await context.Properties
            .Include(p => p.Owner)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<(IEnumerable<Property> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, PropertyFilterDto? filter = null)
    {
        var query = context.Properties
            .Include(p => p.Owner)
            .Include(p => p.Images)
            .Where(p => userId == null || p.OwnerId == userId)
            .Where(p => filter == null || filter.Type == null || p.Type == filter.Type)
            .Where(p => filter == null || filter.Location == null || p.Location.ToLower().Contains(filter.Location.ToLower()))
            .Where(p => filter == null || filter.MinPrice == null || p.Price >= filter.MinPrice)
            .Where(p => filter == null || filter.MaxPrice == null || p.Price <= filter.MaxPrice)
            .Where(p => filter == null || filter.MinBedrooms == null || p.NumberOfBedrooms >= filter.MinBedrooms)
            .Where(p => filter == null || filter.MaxBedrooms == null || p.NumberOfBedrooms <= filter.MaxBedrooms)
            .OrderByDescending(p => p.CreatedAt);
        int totalCount = await query.CountAsync();
        List<Property> items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task UpdateAsync(Property property)
    {
        context.Properties.Update(property);
        await context.SaveChangesAsync();
    }
}
