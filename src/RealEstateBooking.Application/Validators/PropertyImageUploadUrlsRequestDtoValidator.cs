using FluentValidation;

using RealEstateBooking.Application.DTOs.PropertyImage;

namespace RealEstateBooking.Application.Validators;

public class PropertyImageUploadUrlsRequestDtoValidator : AbstractValidator<PropertyImageUploadUrlsRequestDto>
{
    public PropertyImageUploadUrlsRequestDtoValidator()
    {
        RuleFor(x => x.Images)
            .NotEmpty().WithMessage("At least one image is required.")
            .Must(list => list.Count <= 20).WithMessage("Cannot upload more than 20 images at once.");

        RuleForEach(x => x.Images).SetValidator(new PropertyImageUploadItemRequestDtoValidator());
    }
}
