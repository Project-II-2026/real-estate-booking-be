using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Property;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Application.Mappers;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.Application.Services;

public class PropertyService(
    IPropertyRepository propertyRepository,
    IUserRepository userRepository,
    IS3Service s3Service
) : IPropertyService
{
    public async Task<PropertyResponseDto> CreateAsync(PropertyCreateRequestDto request, int ownerId)
    {
        var owner = await userRepository.GetByIdAsync(ownerId)
                    ?? throw new NotFoundException("User not found.");

        var property = PropertyMapper.FromPropertyCreateRequestDtoToProperty(request, owner);

        await propertyRepository.AddAsync(property);

        return PropertyMapper.FromPropertyToPropertyResponseDto(property, s3Service.GetObjectUrl);
    }

    public async Task<PropertyResponseDto> GetByIdAsync(int id)
    {
        var property = await propertyRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException($"Property with id {id} was not found.");

        return PropertyMapper.FromPropertyToPropertyResponseDto(property, s3Service.GetObjectUrl);
    }

    public async Task<PaginationResponseDto<PropertyResponseDto>> GetAllAsync(PaginationRequestDto parameters, PropertyFilterDto? filter = null)
    {
        var (items, totalCount) = await propertyRepository.GetPagedAsync(parameters.Page, parameters.PageSize, filter: filter);
        var dtos = items.Select(p => PropertyMapper.FromPropertyToPropertyResponseDto(p, s3Service.GetObjectUrl));
        return PaginationMapper.FromPagedResultToPaginationResponseDto(dtos, parameters.Page, parameters.PageSize, totalCount);
    }

    public async Task<PaginationResponseDto<PropertyResponseDto>> GetByUserAsync(int userId, PaginationRequestDto parameters, PropertyFilterDto? filter = null)
    {
        _ = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var (items, totalCount) = await propertyRepository.GetPagedAsync(parameters.Page, parameters.PageSize, userId, filter);
        var dtos = items.Select(p => PropertyMapper.FromPropertyToPropertyResponseDto(p, s3Service.GetObjectUrl));
        return PaginationMapper.FromPagedResultToPaginationResponseDto(dtos, parameters.Page, parameters.PageSize, totalCount);
    }

    public async Task<PropertyResponseDto> UpdateAsync(int id, PropertyUpdateRequestDto request, int requestingUserId)
    {
        var property = await propertyRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException($"Property with id {id} was not found.");

        if (property.OwnerId != requestingUserId)
            throw new ForbiddenException("You are not the owner of this property.");

        PropertyMapper.FromPropertyUpdateRequestDtoToProperty(request, property);

        await propertyRepository.UpdateAsync(property);

        return PropertyMapper.FromPropertyToPropertyResponseDto(property, s3Service.GetObjectUrl);
    }
}
