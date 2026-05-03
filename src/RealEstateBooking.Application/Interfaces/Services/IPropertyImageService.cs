using RealEstateBooking.Application.DTOs.PropertyImage;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IPropertyImageService
{
    Task<List<PropertyImagePresignedUrlResponseDto>> GenerateUploadUrlsAsync(int propertyId, PropertyImageUploadUrlsRequestDto request, int requestingUserId);
    Task<PropertyImageStatusResponseDto> CompleteUploadAsync(int propertyId, int imageId, int requestingUserId);
    Task<PropertyImageStatusResponseDto> FailUploadAsync(int propertyId, int imageId, int requestingUserId);
}
