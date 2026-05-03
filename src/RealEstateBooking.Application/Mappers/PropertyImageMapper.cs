using Mapster;

using RealEstateBooking.Application.DTOs.PropertyImage;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.Application.Mappers;

public class PropertyImageMapper
{
    public static PropertyImagePresignedUrlResponseDto FromPropertyImageToPresignedUrlResponseDto(
        PropertyImage image, string presignedUrl, Dictionary<string, string> requiredHeaders)
    {
        var dto = image.Adapt<PropertyImagePresignedUrlResponseDto>();
        dto.PresignedUrl = presignedUrl;
        dto.RequiredHeaders = requiredHeaders;
        return dto;
    }

    public static PropertyImageStatusResponseDto FromPropertyImageToStatusResponseDto(
        PropertyImage image, string url, PropertyImageStatus status)
    {
        return new PropertyImageStatusResponseDto
        {
            ImageId = image.Id,
            Url = url,
            Status = status.ToString()
        };
    }

    public static void Configure()
    {
        TypeAdapterConfig<PropertyImage, PropertyImagePresignedUrlResponseDto>.NewConfig()
            .Map(dest => dest.ImageId, src => src.Id)
            .Ignore(dest => dest.PresignedUrl)
            .Ignore(dest => dest.RequiredHeaders);
    }
}
