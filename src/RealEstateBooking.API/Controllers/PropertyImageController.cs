using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RealEstateBooking.Application.DTOs.PropertyImage;
using RealEstateBooking.Application.Interfaces.Services;

namespace RealEstateBooking.API.Controllers;

[Route("/properties/{propertyId:int}/images")]
public class PropertyImageController(IPropertyImageService propertyImageService) : BaseController
{
    [HttpPost("upload-urls")]
    [Authorize]
    public async Task<ActionResult<List<PropertyImagePresignedUrlResponseDto>>> GenerateUploadUrls(
        int propertyId, PropertyImageUploadUrlsRequestDto dto)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        List<PropertyImagePresignedUrlResponseDto> result = await propertyImageService.GenerateUploadUrlsAsync(propertyId, dto, userId);
        return StatusCode(201, result);
    }

    [HttpPatch("{imageId:int}/complete")]
    [Authorize]
    public async Task<ActionResult<PropertyImageStatusResponseDto>> CompleteUpload(int propertyId, int imageId)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        PropertyImageStatusResponseDto result = await propertyImageService.CompleteUploadAsync(propertyId, imageId, userId);
        return Ok(result);
    }

    [HttpPatch("{imageId:int}/fail")]
    [Authorize]
    public async Task<ActionResult<PropertyImageStatusResponseDto>> FailUpload(int propertyId, int imageId)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        PropertyImageStatusResponseDto result = await propertyImageService.FailUploadAsync(propertyId, imageId, userId);
        return Ok(result);
    }
}
