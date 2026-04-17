using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    RefreshToken GenerateRefreshToken(int userId);
}