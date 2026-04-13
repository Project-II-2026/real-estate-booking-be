using Microsoft.EntityFrameworkCore;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Infrastructure.Persistance;

namespace RealEstateBooking.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetAsync(string token)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(
            refreshToken => refreshToken.Token == token
            );
    }
}