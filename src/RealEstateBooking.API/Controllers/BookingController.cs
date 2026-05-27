using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.Interfaces.Services;

namespace RealEstateBooking.API.Controllers;

[Route("/bookings")]
public class BookingController(IBookingService bookingService) : BaseController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BookingResponseDto>> Create(BookingCreateRequestDto dto)
    {
        var visitorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await bookingService.CreateAsync(dto, visitorId);
        return StatusCode(201, result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<PaginationResponseDto<BookingResponseDto>>> GetMine(
        [FromQuery] PaginationRequestDto parameters,
        [FromQuery] BookingFilterDto filter)
    {
        var visitorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await bookingService.GetMineAsync(visitorId, parameters, filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<BookingResponseDto>> GetById(int id)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await bookingService.GetByIdAsync(id, requestingUserId);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await bookingService.CancelAsync(id, requestingUserId);
        return NoContent();
    }
}
