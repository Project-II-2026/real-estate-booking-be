using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateBooking.Application.DTOs.Auth;
using RealEstateBooking.Application.DTOs.User;
using RealEstateBooking.Application.Interfaces.Services;

namespace RealEstateBooking.API.Controllers;

[Route("/users")]
public class UserController(IUserService userService) : BaseController
{
    [HttpPost("register")]
    public async Task<ActionResult<RegistrationResponseDto>> Register(RegistrationRequestDto dto)
    {
        var result = await userService.RegisterAsync(dto);
        return CreatedAtAction(nameof(Register), result);
    }
    
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        var result = await userService.GetByIdAsync(id);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
    
        var user = await userService.GetMeAsync(User);
        return Ok(user);
    }
}