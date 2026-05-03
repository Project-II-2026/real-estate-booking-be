using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Property;

namespace RealEstateBooking.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<PropertyResponseDto> CreateAsync(PropertyCreateRequestDto request, int ownerId);
    Task<PropertyResponseDto> GetByIdAsync(int id);
    Task<PaginationResponseDto<PropertyResponseDto>> GetAllAsync(PaginationRequestDto parameters, PropertyFilterDto? filter = null);
    Task<PaginationResponseDto<PropertyResponseDto>> GetByUserAsync(int userId, PaginationRequestDto parameters, PropertyFilterDto? filter = null);
    Task<PropertyResponseDto> UpdateAsync(int id, PropertyUpdateRequestDto request, int requestingUserId);
}
