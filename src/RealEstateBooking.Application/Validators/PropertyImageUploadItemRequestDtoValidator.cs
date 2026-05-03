using FluentValidation;

using RealEstateBooking.Application.DTOs.PropertyImage;

namespace RealEstateBooking.Application.Validators;

public class PropertyImageUploadItemRequestDtoValidator : AbstractValidator<PropertyImageUploadItemRequestDto>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];

    public PropertyImageUploadItemRequestDtoValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than 0.")
            .LessThanOrEqualTo(10_485_760).WithMessage("File size must not exceed 10 MB.");
        RuleFor(x => x.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Content type must be image/jpeg, image/png, or image/webp.");
    }
}
