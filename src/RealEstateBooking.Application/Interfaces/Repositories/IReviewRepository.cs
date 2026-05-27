using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(Review review);
    Task<Review?> GetByIdAsync(int id);
    Task<bool> HasUserReviewedPropertyAsync(int propertyId, int reviewerId);
    Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedForPropertyAsync(int propertyId, int page, int pageSize);
    Task<double> GetAverageRatingAsync(int propertyId);
}
