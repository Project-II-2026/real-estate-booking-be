using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Property;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Domain.Enums;

namespace RealEstateBooking.API.Controllers;

[Route("/properties")]
public class PropertyController(
    IPropertyService propertyService,
    IBookingService bookingService
) : BaseController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PropertyResponseDto>> Create(PropertyCreateRequestDto dto)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await propertyService.CreateAsync(dto, ownerId);
        return StatusCode(201, result);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginationResponseDto<PropertyResponseDto>>> GetAll(
        [FromQuery] PaginationRequestDto parameters,
        [FromQuery] int? userId,
        [FromQuery] PropertyFilterDto filter)
    {
        var result = userId.HasValue
            ? await propertyService.GetByUserAsync(userId.Value, parameters, filter)
            : await propertyService.GetAllAsync(parameters, filter);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<PaginationResponseDto<PropertyResponseDto>>> GetMine(
        [FromQuery] PaginationRequestDto parameters,
        [FromQuery] PropertyFilterDto filter)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await propertyService.GetByUserAsync(userId, parameters, filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<PropertyResponseDto>> GetById(int id)
    {
        var result = await propertyService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<PropertyResponseDto>> Update(int id, PropertyUpdateRequestDto dto)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        bool isAdmin = User.IsInRole(nameof(UserRole.SuperAdmin));
        var result = await propertyService.UpdateAsync(id, dto, requestingUserId, isAdmin);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        bool isAdmin = User.IsInRole(nameof(UserRole.SuperAdmin));
        await propertyService.DeleteAsync(id, requestingUserId, isAdmin);
        return NoContent();
    }

    [HttpGet("{id:int}/bookings")]
    [Authorize]
    public async Task<ActionResult<PaginationResponseDto<BookingResponseDto>>> GetBookings(
        int id,
        [FromQuery] PaginationRequestDto parameters,
        [FromQuery] BookingFilterDto filter)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        bool isAdmin = User.IsInRole(nameof(UserRole.SuperAdmin));
        var result = await bookingService.GetForPropertyAsync(id, requestingUserId, parameters, filter, isAdmin);
        return Ok(result);
    }

    [HttpGet("{id:int}/availability")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<BookingSlotDto>>> GetAvailability(int id)
    {
        var result = await bookingService.GetAvailabilityAsync(id);
        return Ok(result);
    }
}
