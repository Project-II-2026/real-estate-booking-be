using FluentValidation;

using RealEstateBooking.Application.DTOs.Auth;

namespace RealEstateBooking.Application.Validators;

public class RegistrationRequestDtoValidator : AbstractValidator<RegistrationRequestDto>
{
    public RegistrationRequestDtoValidator()
    {
        RuleFor(dto => dto.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Matches(@".*\p{L}.*").WithMessage("Username must contain at least one letter.");

        RuleFor(dto => dto.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Matches(@".*\p{L}.*").WithMessage("First name must contain at least one letter.");

        RuleFor(dto => dto.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Matches(@".*\p{L}.*").WithMessage("Last name must contain at least one letter.");

        RuleFor(dto => dto.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");

        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(dto => dto.PasswordConfirmation)
            .Equal(dto => dto.Password).WithMessage("Passwords do not match.");
    }
}
