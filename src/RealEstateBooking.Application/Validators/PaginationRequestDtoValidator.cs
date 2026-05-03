using FluentValidation;

using RealEstateBooking.Application.DTOs.Common;

namespace RealEstateBooking.Application.Validators;

public class PaginationRequestDtoValidator : AbstractValidator<PaginationRequestDto>
{
    public PaginationRequestDtoValidator()
    {
        RuleFor(p => p.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(p => p.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");
    }
}
