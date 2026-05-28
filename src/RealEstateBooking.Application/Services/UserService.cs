using System.Security.Claims;

using Microsoft.Extensions.Logging;
using RealEstateBooking.Application.DTOs.Auth;
using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.User;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Application.Mappers;
using RealEstateBooking.Domain.Enums;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ILogger<UserService> logger
) : IUserService
{
    public async Task<RegistrationResponseDto> RegisterAsync(RegistrationRequestDto request)
    {
        if (await userRepository.ExistsByEmailOrUsernameAsync(request.Email, request.Username))
            throw new ConflictException("Email or username is already in use.");

        var role = await roleRepository.GetByNameAsync(nameof(UserRole.User))
                   ?? throw new NotFoundException("Default role not found.");

        var user = UserMapper.FromRegistrationRequestDtoToUser(request, role);

        await userRepository.AddAsync(user);

        return UserMapper.FromUserToRegistrationResponseDto(user, role);
    }

    public async Task<UserResponseDto> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id)
                   ?? throw new NotFoundException($"User with id {id} was not found.");

        return UserMapper.FromUserToUserResponseDto(user);
    }

    public async Task<UserResponseDto> GetMeAsync(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
                          ?? throw new UnauthorizedException("User is not authenticated.");

        var user = await userRepository.GetByIdAsync(int.Parse(userIdClaim.Value))
                   ?? throw new NotFoundException("User not found.");

        return UserMapper.FromUserToUserResponseDto(user);
    }

    public async Task AdminDeleteAsync(int id, int currentUserId)
    {
        if (id == currentUserId)
            throw new BadRequestException("SuperAdmins cannot delete their own account.");

        var user = await userRepository.GetByIdAsync(id)
                   ?? throw new NotFoundException($"User with id {id} was not found.");

        await userRepository.DeleteAsync(user);

        logger.LogInformation("User {UserId} deleted by admin {AdminId}.", id, currentUserId);
    }

    public async Task<PaginationResponseDto<UserResponseDto>> AdminGetAllAsync(PaginationRequestDto parameters)
    {
        var (items, totalCount) = await userRepository.GetPagedAsync(parameters.Page, parameters.PageSize);
        var dtos = items.Select(UserMapper.FromUserToUserResponseDto);
        return PaginationMapper.FromPagedResultToPaginationResponseDto(dtos, parameters.Page, parameters.PageSize, totalCount);
    }
}
