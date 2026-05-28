using RealEstateBooking.Application.DTOs.PropertyImage;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IPropertyImageService
{
    Task<List<PropertyImagePresignedUrlResponseDto>> GenerateUploadUrlsAsync(int propertyId, PropertyImageUploadUrlsRequestDto request, int requestingUserId, bool isAdmin = false);
    Task<PropertyImageStatusResponseDto> CompleteUploadAsync(int propertyId, int imageId, int requestingUserId, bool isAdmin = false);
    Task<PropertyImageStatusResponseDto> FailUploadAsync(int propertyId, int imageId, int requestingUserId, bool isAdmin = false);
}
