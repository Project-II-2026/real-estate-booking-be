using Mapster;

using RealEstateBooking.Application.DTOs.Review;
using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Mappers;

public class ReviewMapper
{
    public static ReviewResponseDto FromReviewToReviewResponseDto(Review review) =>
        review.Adapt<ReviewResponseDto>();

    public static Review FromReviewCreateRequestDtoToReview(ReviewCreateRequestDto dto) =>
        dto.Adapt<Review>();

    public static void Configure()
    {
        TypeAdapterConfig<ReviewCreateRequestDto, Review>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.Version)
            .Ignore(dest => dest.ReviewerId)
            .Ignore(dest => dest.Reviewer)
            .Ignore(dest => dest.Property);

        TypeAdapterConfig<Review, ReviewResponseDto>.NewConfig()
            .Map(dest => dest.PropertyTitle, src => src.Property.Title)
            .Map(dest => dest.ReviewerUsername, src => src.Reviewer.Username);
    }
}
