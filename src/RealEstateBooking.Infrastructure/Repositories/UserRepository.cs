using Microsoft.EntityFrameworkCore;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Infrastructure.Persistance;

namespace RealEstateBooking.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Username == username);
    }
    
    public async Task<bool> ExistsByEmailOrUsernameAsync(string email, string username)
    {
        return await context.Users.AnyAsync(user => user.Email == email || user.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}