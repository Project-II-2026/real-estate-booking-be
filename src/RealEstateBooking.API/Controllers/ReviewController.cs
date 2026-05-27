using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Review;
using RealEstateBooking.Application.Interfaces.Services;

namespace RealEstateBooking.API.Controllers;

[Route("/reviews")]
public class ReviewController(IReviewService reviewService) : BaseController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewResponseDto>> Create(ReviewCreateRequestDto dto)
    {
        var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await reviewService.CreateAsync(dto, reviewerId);
        return StatusCode(201, result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ReviewResponseDto>> Update(int id, ReviewUpdateRequestDto dto)
    {
        var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await reviewService.UpdateAsync(id, dto, reviewerId);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await reviewService.DeleteAsync(id, reviewerId);
        return NoContent();
    }

    [HttpGet("property/{propertyId:int}")]
    [Authorize]
    public async Task<ActionResult<PropertyReviewsResponseDto>> GetForProperty(
        int propertyId,
        [FromQuery] PaginationRequestDto parameters)
    {
        var result = await reviewService.GetForPropertyAsync(propertyId, parameters);
        return Ok(result);
    }
}
