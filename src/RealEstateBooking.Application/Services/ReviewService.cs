using Microsoft.Extensions.Logging;

using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Review;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Application.Mappers;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.Application.Services;

public class ReviewService(
    IReviewRepository reviewRepository,
    IBookingRepository bookingRepository,
    IPropertyRepository propertyRepository,
    ILogger<ReviewService> logger
) : IReviewService
{
    public async Task<ReviewResponseDto> CreateAsync(ReviewCreateRequestDto request, int reviewerId)
    {
        var property = await propertyRepository.GetByIdAsync(request.PropertyId)
                       ?? throw new NotFoundException($"Property with id {request.PropertyId} was not found.");

        if (property.OwnerId == reviewerId)
            throw new ForbiddenException("You cannot review your own property.");

        if (!await bookingRepository.HasCompletedBookingAsync(property.Id, reviewerId))
            throw new ForbiddenException("You can only review properties you have visited.");

        if (await reviewRepository.HasUserReviewedPropertyAsync(property.Id, reviewerId))
            throw new ConflictException("You have already reviewed this property.");

        var review = ReviewMapper.FromReviewCreateRequestDtoToReview(request);
        review.ReviewerId = reviewerId;
        review.PropertyId = property.Id;

        await reviewRepository.AddAsync(review);

        var created = await reviewRepository.GetByIdAsync(review.Id)
                      ?? throw new NotFoundException("Review not found.");

        logger.LogInformation("Review {ReviewId} created by user {ReviewerId} on property {PropertyId}.",
            created.Id, reviewerId, property.Id);

        return ReviewMapper.FromReviewToReviewResponseDto(created);
    }

    public async Task<ReviewResponseDto> UpdateAsync(int id, ReviewUpdateRequestDto request, int reviewerId)
    {
        var review = await reviewRepository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Review with id {id} was not found.");

        if (review.ReviewerId != reviewerId)
            throw new ForbiddenException("You can only edit your own review.");

        review.Rating = request.Rating;
        review.Comment = request.Comment;

        await reviewRepository.UpdateAsync(review);

        logger.LogInformation("Review {ReviewId} updated by user {ReviewerId}.", review.Id, reviewerId);

        return ReviewMapper.FromReviewToReviewResponseDto(review);
    }

    public async Task DeleteAsync(int id, int reviewerId)
    {
        var review = await reviewRepository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Review with id {id} was not found.");

        if (review.ReviewerId != reviewerId)
            throw new ForbiddenException("You can only delete your own review.");

        await reviewRepository.DeleteAsync(review);

        logger.LogInformation("Review {ReviewId} deleted by user {ReviewerId}.", id, reviewerId);
    }

    public async Task<PropertyReviewsResponseDto> GetForPropertyAsync(int propertyId, PaginationRequestDto parameters)
    {
        _ = await propertyRepository.GetByIdAsync(propertyId)
            ?? throw new NotFoundException($"Property with id {propertyId} was not found.");

        var (items, totalCount) = await reviewRepository.GetPagedForPropertyAsync(propertyId, parameters.Page, parameters.PageSize);
        double average = totalCount == 0 ? 0d : await reviewRepository.GetAverageRatingAsync(propertyId);

        var dtos = items.Select(ReviewMapper.FromReviewToReviewResponseDto).ToList();
        int totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        return new PropertyReviewsResponseDto
        {
            Items = dtos,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = parameters.Page < totalPages,
            HasPreviousPage = parameters.Page > 1,
            AverageRating = average
        };
    }
}
