using RealEstateBooking.Application.DTOs.Property;
using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task AddAsync(Property property);
    Task<Property?> GetByIdAsync(int id);
    Task<(IEnumerable<Property> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, PropertyFilterDto? filter = null);
    Task UpdateAsync(Property property);
}
