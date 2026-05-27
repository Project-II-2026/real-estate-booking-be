using FluentValidation;

using RealEstateBooking.Application.DTOs.Review;

namespace RealEstateBooking.Application.Validators;

public class ReviewCreateRequestDtoValidator : AbstractValidator<ReviewCreateRequestDto>
{
    public ReviewCreateRequestDtoValidator()
    {
        RuleFor(dto => dto.PropertyId).GreaterThan(0).WithMessage("PropertyId is required.");
        RuleFor(dto => dto.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        RuleFor(dto => dto.Comment).MaximumLength(1000).WithMessage("Comment must be 1000 characters or fewer.");
    }
}
