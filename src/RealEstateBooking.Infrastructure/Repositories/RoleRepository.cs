using Microsoft.EntityFrameworkCore;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Infrastructure.Persistance;

namespace RealEstateBooking.Infrastructure.Repositories;

public class RoleRepository(AppDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name) =>
        await context.Roles.FirstOrDefaultAsync(r => r.Name == name);
}