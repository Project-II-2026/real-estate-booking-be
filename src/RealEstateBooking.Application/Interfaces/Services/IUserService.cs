using RealEstateBooking.Application.DTOs.Auth;
using RealEstateBooking.Application.DTOs.User;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IUserService
{
    Task <RegistrationResponseDto> RegisterAsync(RegistrationRequestDto request);
    
    Task<UserResponseDto> GetByIdAsync(int id);
}