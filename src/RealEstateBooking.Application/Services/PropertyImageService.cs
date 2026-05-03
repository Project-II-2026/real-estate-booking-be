using RealEstateBooking.Application.DTOs.PropertyImage;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Application.Mappers;
using RealEstateBooking.Domain.Entities;
using RealEstateBooking.Domain.Enums;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.Application.Services;

public class PropertyImageService(
    IPropertyRepository propertyRepository,
    IPropertyImageRepository propertyImageRepository,
    IS3Service s3Service
) : IPropertyImageService
{
    private const int PresignedUrlExpiryMinutes = 15;
    private const int MaxImagesPerProperty = 20;

    public async Task<List<PropertyImagePresignedUrlResponseDto>> GenerateUploadUrlsAsync(
        int propertyId,
        PropertyImageUploadUrlsRequestDto request,
        int requestingUserId)
    {
        var property = await propertyRepository.GetByIdAsync(propertyId)
                       ?? throw new NotFoundException($"Property with id {propertyId} was not found.");

        if (property.OwnerId != requestingUserId)
            throw new ForbiddenException("You are not the owner of this property.");

        await propertyImageRepository.DeleteStalePendingAsync(propertyId, DateTime.UtcNow.AddHours(-1));

        int completedCount = await propertyImageRepository.CountCompletedByPropertyIdAsync(propertyId);
        if (completedCount + request.Images.Count > MaxImagesPerProperty)
            throw new BadRequestException($"Property cannot have more than {MaxImagesPerProperty} images.");

        List<PropertyImage> images = request.Images
            .Select(item => new PropertyImage
            {
                PropertyId = propertyId,
                S3Key = $"properties/{propertyId}/{Guid.NewGuid()}{Path.GetExtension(item.FileName).ToLowerInvariant()}",
                FileName = item.FileName,
                FileSize = item.FileSize,
                ContentType = item.ContentType,
                Status = PropertyImageStatus.Pending
            })
            .ToList();

        await propertyImageRepository.AddRangeAsync(images);

        return images
            .Select(image => PropertyImageMapper.FromPropertyImageToPresignedUrlResponseDto(
                image,
                s3Service.GeneratePresignedPutUrl(image.S3Key, image.ContentType, PresignedUrlExpiryMinutes),
                new Dictionary<string, string> { { "Content-Type", image.ContentType } }))
            .ToList();
    }

    public async Task<PropertyImageStatusResponseDto> CompleteUploadAsync(int propertyId, int imageId, int requestingUserId)
    {
        PropertyImage image = await GetValidatedImageAsync(propertyId, imageId, requestingUserId);

        if (image.Status == PropertyImageStatus.Completed)
            throw new BadRequestException("Image already confirmed.");

        image.Status = PropertyImageStatus.Completed;
        await propertyImageRepository.UpdateAsync(image);

        return PropertyImageMapper.FromPropertyImageToStatusResponseDto(
            image, s3Service.GetObjectUrl(image.S3Key), PropertyImageStatus.Completed);
    }

    public async Task<PropertyImageStatusResponseDto> FailUploadAsync(int propertyId, int imageId, int requestingUserId)
    {
        PropertyImage image = await GetValidatedImageAsync(propertyId, imageId, requestingUserId);

        if (image.Status == PropertyImageStatus.Failed)
            throw new BadRequestException("Image already marked as failed.");

        image.Status = PropertyImageStatus.Failed;
        await propertyImageRepository.UpdateAsync(image);

        return PropertyImageMapper.FromPropertyImageToStatusResponseDto(
            image, string.Empty, PropertyImageStatus.Failed);
    }

    private async Task<PropertyImage> GetValidatedImageAsync(int propertyId, int imageId, int requestingUserId)
    {
        var image = await propertyImageRepository.GetByIdAsync(imageId)
                    ?? throw new NotFoundException($"Image with id {imageId} was not found.");

        if (image.PropertyId != propertyId)
            throw new NotFoundException($"Image with id {imageId} was not found.");

        var property = await propertyRepository.GetByIdAsync(propertyId)
                       ?? throw new NotFoundException($"Property with id {propertyId} was not found.");

        if (property.OwnerId != requestingUserId)
            throw new ForbiddenException("You are not the owner of this property.");

        return image;
    }
}
