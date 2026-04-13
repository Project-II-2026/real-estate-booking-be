using RealEstateBooking.Application.DTOs.Auth;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IUserService
{
    Task <RegistrationResponseDto> RegisterAsync(RegistrationRequestDto request);
}