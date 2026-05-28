using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Property;
using RealEstateBooking.Application.DTOs.User;
using RealEstateBooking.Application.Interfaces.Services;

namespace RealEstateBooking.API.Controllers;

[Route("/admin")]
[Authorize(Roles = "SuperAdmin")]
public class AdminController(
    IPropertyService propertyService,
    IBookingService bookingService,
    IUserService userService
) : BaseController
{
    [HttpPut("properties/{id:int}")]
    public async Task<ActionResult<PropertyResponseDto>> UpdateProperty(int id, PropertyUpdateRequestDto dto)
    {
        var result = await propertyService.AdminUpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<PaginationResponseDto<BookingResponseDto>>> GetAllBookings(
        [FromQuery] PaginationRequestDto parameters,
        [FromQuery] BookingFilterDto filter)
    {
        var result = await bookingService.AdminGetAllAsync(parameters, filter);
        return Ok(result);
    }

    [HttpPut("bookings/{id:int}")]
    public async Task<ActionResult<BookingResponseDto>> UpdateBooking(int id, BookingAdminUpdateRequestDto dto)
    {
        var result = await bookingService.AdminUpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("bookings/{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        await bookingService.AdminDeleteAsync(id);
        return NoContent();
    }

    [HttpGet("users")]
    public async Task<ActionResult<PaginationResponseDto<UserResponseDto>>> GetAllUsers(
        [FromQuery] PaginationRequestDto parameters)
    {
        var result = await userService.AdminGetAllAsync(parameters);
        return Ok(result);
    }

    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await userService.AdminDeleteAsync(id, currentUserId);
        return NoContent();
    }
}
