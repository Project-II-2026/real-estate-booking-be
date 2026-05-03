using Mapster;

using RealEstateBooking.Application.DTOs.Property;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Application.Mappers;

public class PropertyMapper
{
    public static PropertyResponseDto FromPropertyToPropertyResponseDto(Property property, Func<string, string> getObjectUrl)
    {
        var dto = property.Adapt<PropertyResponseDto>();
        dto.Images = property.Images
            .Where(i => i.Status == PropertyImageStatus.Completed)
            .Select(i => getObjectUrl(i.S3Key))
            .ToList();
        return dto;
    }

    public static Property FromPropertyCreateRequestDtoToProperty(PropertyCreateRequestDto dto, User owner)
    {
        var property = dto.Adapt<Property>();
        property.OwnerId = owner.Id;
        property.Owner = owner;
        return property;
    }

    public static void FromPropertyUpdateRequestDtoToProperty(PropertyUpdateRequestDto dto, Property property)
    {
        dto.Adapt(property);
    }

    public static void Configure()
    {
        TypeAdapterConfig<PropertyCreateRequestDto, Property>.NewConfig()
            .Map(dest => dest.Title, src => src.Title.Trim())
            .Map(dest => dest.Description, src => src.Description.Trim())
            .Map(dest => dest.Location, src => src.Location.Trim())
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.Version)
            .Ignore(dest => dest.Owner)
            .Ignore(dest => dest.OwnerId)
            .Ignore(dest => dest.Images);

        TypeAdapterConfig<PropertyUpdateRequestDto, Property>.NewConfig()
            .Map(dest => dest.Title, src => src.Title.Trim())
            .Map(dest => dest.Description, src => src.Description.Trim())
            .Map(dest => dest.Location, src => src.Location.Trim())
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.Version)
            .Ignore(dest => dest.Owner)
            .Ignore(dest => dest.OwnerId)
            .Ignore(dest => dest.Images);

        TypeAdapterConfig<Property, PropertyResponseDto>.NewConfig()
            .Map(dest => dest.OwnerUsername, src => src.Owner.Username)
            .Ignore(dest => dest.Images);
    }
}
