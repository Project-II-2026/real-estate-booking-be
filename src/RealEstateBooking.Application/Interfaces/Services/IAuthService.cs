using RealEstateBooking.Application.DTOs.Auth;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
}