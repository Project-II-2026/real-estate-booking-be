using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Review;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IReviewService
{
    Task<ReviewResponseDto> CreateAsync(ReviewCreateRequestDto request, int reviewerId);
    Task<ReviewResponseDto> UpdateAsync(int id, ReviewUpdateRequestDto request, int reviewerId);
    Task DeleteAsync(int id, int reviewerId);
    Task<PropertyReviewsResponseDto> GetForPropertyAsync(int propertyId, PaginationRequestDto parameters);
}
