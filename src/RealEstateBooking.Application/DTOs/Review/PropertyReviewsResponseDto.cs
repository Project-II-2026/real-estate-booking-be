using RealEstateBooking.Application.DTOs.Common;

namespace RealEstateBooking.Application.DTOs.Review;

public class PropertyReviewsResponseDto : PaginationResponseDto<ReviewResponseDto>
{
    public double AverageRating { get; set; }
}
