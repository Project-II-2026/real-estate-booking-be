using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Repositories;

public interface IPropertyImageRepository
{
    Task AddRangeAsync(IEnumerable<PropertyImage> images);
    Task<PropertyImage?> GetByIdAsync(int id);
    Task<int> CountCompletedByPropertyIdAsync(int propertyId);
    Task UpdateAsync(PropertyImage image);
    Task DeleteStalePendingAsync(int propertyId, DateTime olderThan);
}
