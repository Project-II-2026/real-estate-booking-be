using FluentValidation;

using RealEstateBooking.Application.DTOs.Property;

namespace RealEstateBooking.Application.Validators;

public class PropertyUpdateRequestDtoValidator : AbstractValidator<PropertyUpdateRequestDto>
{
    public PropertyUpdateRequestDtoValidator()
    {
        RuleFor(dto => dto.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(dto => dto.Location).NotEmpty().WithMessage("Location is required.");
        RuleFor(dto => dto.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(dto => dto.SizeInSquareMeters).GreaterThan(0).WithMessage("Size must be greater than zero.");
        RuleFor(dto => dto.NumberOfBedrooms).GreaterThanOrEqualTo(0).WithMessage("Number of bedrooms cannot be negative.");
        RuleFor(dto => dto.NumberOfBathrooms).GreaterThanOrEqualTo(0).WithMessage("Number of bathrooms cannot be negative.");
    }
}
