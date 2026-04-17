using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
}