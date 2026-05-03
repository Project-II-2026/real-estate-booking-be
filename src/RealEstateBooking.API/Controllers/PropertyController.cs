using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.DTOs.Property;
using RealEstateBooking.Application.Interfaces.Services;

namespace RealEstateBooking.API.Controllers;

[Route("/properties")]
public class PropertyController(IPropertyService propertyService) : BaseController
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
        var result = await propertyService.UpdateAsync(id, dto, requestingUserId);
        return Ok(result);
    }
}
