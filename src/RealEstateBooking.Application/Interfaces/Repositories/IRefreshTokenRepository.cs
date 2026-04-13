using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetAsync(string token);
    Task<RefreshToken?> UpdateAsync(RefreshToken token);
}