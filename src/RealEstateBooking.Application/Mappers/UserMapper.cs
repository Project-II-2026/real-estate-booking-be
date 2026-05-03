using Mapster;

using RealEstateBooking.Application.DTOs.Auth;
using RealEstateBooking.Application.DTOs.User;
using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Mappers;

public class UserMapper
{
    public static User FromRegistrationRequestDtoToUser(RegistrationRequestDto dto, Role role)
    {
        var user = dto.Adapt<User>();
        user.RoleId = role.Id;
        return user;
    }

    public static RegistrationResponseDto FromUserToRegistrationResponseDto(User user, Role role) =>
        new RegistrationResponseDto
        {
            Username = user.Username,
            Email = user.Email,
            Role = role.Name
        };

    public static UserResponseDto FromUserToUserResponseDto(User user) =>
        user.Adapt<UserResponseDto>();

    public static void Configure()
    {
        TypeAdapterConfig<RegistrationRequestDto, User>.NewConfig()
            .Map(dest => dest.Username, src => src.Username.ToLower())
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Email, src => src.Email.ToLower())
            .Map(dest => dest.PasswordHash, src => BCrypt.Net.BCrypt.HashPassword(src.Password))
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.Version)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Role)
            .Ignore(dest => dest.RoleId);

        TypeAdapterConfig<User, RegistrationResponseDto>.NewConfig()
            .Map(dest => dest.Role, src => src.Role.Name);

        TypeAdapterConfig<User, UserResponseDto>.NewConfig()
            .Map(dest => dest.Role, src => src.Role.Name);
    }
}
