using Microsoft.EntityFrameworkCore;

using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Domain.Enums;
using RealEstateBooking.Infrastructure.Persistance;

namespace RealEstateBooking.Infrastructure.Repositories;

public class PropertyImageRepository(AppDbContext context) : IPropertyImageRepository
{
    public async Task AddRangeAsync(IEnumerable<PropertyImage> images)
    {
        await context.PropertyImages.AddRangeAsync(images);
        await context.SaveChangesAsync();
    }

    public async Task<PropertyImage?> GetByIdAsync(int id) =>
        await context.PropertyImages.FirstOrDefaultAsync(i => i.Id == id);

    public async Task<int> CountCompletedByPropertyIdAsync(int propertyId) =>
        await context.PropertyImages.CountAsync(i =>
            i.PropertyId == propertyId && i.Status == PropertyImageStatus.Completed);

    public async Task UpdateAsync(PropertyImage image)
    {
        context.PropertyImages.Update(image);
        await context.SaveChangesAsync();
    }

    public async Task DeleteStalePendingAsync(int propertyId, DateTime olderThan)
    {
        List<PropertyImage> stale = await context.PropertyImages
            .Where(i => i.PropertyId == propertyId
                        && i.Status == PropertyImageStatus.Pending
                        && i.CreatedAt < olderThan)
            .ToListAsync();

        if (stale.Count > 0)
        {
            context.PropertyImages.RemoveRange(stale);
            await context.SaveChangesAsync();
        }
    }
}
