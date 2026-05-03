using RealEstateBooking.Application.DTOs.Auth;

namespace RealEstateBooking.Application.Mappers;

public static class AuthMapper
{
    public static LoginResponseDto FromTokensToLoginResponseDto(string accessToken, string refreshToken) =>
        new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
}
