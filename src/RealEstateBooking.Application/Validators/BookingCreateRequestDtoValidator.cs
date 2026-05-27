using FluentValidation;

using RealEstateBooking.Application.DTOs.Booking;

namespace RealEstateBooking.Application.Validators;

public class BookingCreateRequestDtoValidator : AbstractValidator<BookingCreateRequestDto>
{
    public BookingCreateRequestDtoValidator()
    {
        RuleFor(dto => dto.PropertyId).GreaterThan(0).WithMessage("PropertyId is required.");
        RuleFor(dto => dto.StartTime).NotEqual(default(DateTime)).WithMessage("Start time is required.");
    }
}
