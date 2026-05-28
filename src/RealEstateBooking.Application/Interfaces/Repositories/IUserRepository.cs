using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByEmailOrUsernameAsync(string email, string username);
    Task AddAsync(User user);
    Task DeleteAsync(User user);
    Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
}