using System.Security.Claims;
using Mapster;
using RealEstateBooking.Application.DTOs.Auth;
using RealEstateBooking.Application.DTOs.User;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Domain.Enums;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.Application.Services;

public class UserService(
    IUserRepository userRepository, 
    IRoleRepository roleRepository) : IUserService
{
    public async Task<RegistrationResponseDto> RegisterAsync(RegistrationRequestDto request)
    {
        if (request.Password != request.PasswordConfirmation)
            throw new BadRequestException("Passwords do not match.");

        if (await userRepository.ExistsByEmailOrUsernameAsync(request.Email, request.Username))
            throw new ConflictException("Email or username is already in use.");

        var role = await roleRepository.GetByNameAsync(nameof(UserRole.User))
                   ?? throw new NotFoundException("Default role not found.");

        var user = request.Adapt<User>();
        user.RoleId = role.Id;

        await userRepository.AddAsync(user);

        return new RegistrationResponseDto
        {
            Username = user.Username,
            Email = user.Email,
            Role = role.Name
        };
    }

    public async Task<UserResponseDto> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id)
                   ?? throw new NotFoundException($"User with id {id} was not found.");

        return user.Adapt<UserResponseDto>();
    }

    public async Task<UserResponseDto> GetMeAsync(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
                          ?? throw new UnauthorizedException("User is not authenticated.");

        var user = await userRepository.GetByIdAsync(int.Parse(userIdClaim.Value))
                   ?? throw new NotFoundException("User not found.");

        return user.Adapt<UserResponseDto>();
    }
}