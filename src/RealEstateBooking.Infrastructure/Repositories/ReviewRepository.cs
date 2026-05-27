using Microsoft.EntityFrameworkCore;

using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Infrastructure.Persistance;

namespace RealEstateBooking.Infrastructure.Repositories;

public class ReviewRepository(AppDbContext context) : IReviewRepository
{
    public async Task AddAsync(Review review)
    {
        await context.Reviews.AddAsync(review);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        context.Reviews.Update(review);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        context.Reviews.Remove(review);
        await context.SaveChangesAsync();
    }

    public async Task<Review?> GetByIdAsync(int id) =>
        await context.Reviews
            .Include(r => r.Property)
            .Include(r => r.Reviewer)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> HasUserReviewedPropertyAsync(int propertyId, int reviewerId) =>
        await context.Reviews
            .AnyAsync(r => r.PropertyId == propertyId && r.ReviewerId == reviewerId);

    public async Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedForPropertyAsync(int propertyId, int page, int pageSize)
    {
        var query = context.Reviews
            .Include(r => r.Property)
            .Include(r => r.Reviewer)
            .Where(r => r.PropertyId == propertyId)
            .OrderByDescending(r => r.CreatedAt);

        int totalCount = await query.CountAsync();
        List<Review> items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<double> GetAverageRatingAsync(int propertyId) =>
        await context.Reviews
            .Where(r => r.PropertyId == propertyId)
            .Select(r => (double?)r.Rating)
            .AverageAsync() ?? 0d;
}
