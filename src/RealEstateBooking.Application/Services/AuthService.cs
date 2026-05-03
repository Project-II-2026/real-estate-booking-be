using RealEstateBooking.Application.DTOs.Auth;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Application.Mappers;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService
    ) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email)
                   ?? throw new UnauthorizedException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var accessToken = tokenService.GenerateJwtToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.AddAsync(refreshToken);

        return AuthMapper.FromTokensToLoginResponseDto(accessToken, refreshToken.Token);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string token)
    {
        var refreshToken = await refreshTokenRepository.GetAsync(token)
            ?? throw new NotFoundException("Could not find refresh token.");
        var user = await userRepository.GetByIdAsync(refreshToken.UserId)
            ?? throw new NotFoundException("Could not find user.");

        var newAccessToken = tokenService.GenerateJwtToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.AddAsync(newRefreshToken);

        return AuthMapper.FromTokensToLoginResponseDto(newAccessToken, newRefreshToken.Token);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = await refreshTokenRepository.GetAsync(refreshToken);
        if (token is not null)
        {
            token.IsRevoked = true;
            await refreshTokenRepository.UpdateAsync(token);
        }
    }
}
