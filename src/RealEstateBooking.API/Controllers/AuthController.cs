using Microsoft.AspNetCore.Mvc;
using RealEstateBooking.Application.DTOs.Auth;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.API.Controllers;

[Route("/auth")]
public class AuthController(
    IAuthService authService,
    IConfiguration configuration,
    IWebHostEnvironment env
) : BaseController
{

    [HttpPost("login")]
    public async Task<NoContentResult> Login(LoginRequestDto dto)
    {
        var response = await authService.LoginAsync(dto);

        AppendTokenCookie("accessToken", response.AccessToken, 
            DateTimeOffset.UtcNow.AddMinutes(double.Parse(configuration["Jwt:ExpiresInMinutes"]!)));
    
        AppendTokenCookie("refreshToken", response.RefreshToken, 
            DateTimeOffset.UtcNow.AddDays(double.Parse(configuration["RefreshToken:ExpiresInDays"]!)));

        return NoContent();
    }
    
    [HttpPost("refresh")]
    public async Task<NoContentResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"]
                           ?? throw new UnauthorizedException("Refresh token not found in cookie.");
        var response = await authService.RefreshTokenAsync(refreshToken);

        AppendTokenCookie("accessToken", response.AccessToken, 
            DateTimeOffset.UtcNow.AddMinutes(double.Parse(configuration["Jwt:ExpiresInMinutes"]!)));
    
        AppendTokenCookie("refreshToken", response.RefreshToken, 
            DateTimeOffset.UtcNow.AddDays(double.Parse(configuration["RefreshToken:ExpiresInDays"]!)));

        return NoContent();
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {   
        // TODO: Add call to service to invalidate refresh token
        // await authService.RefreshTokenAsync(refreshToken);
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    private void AppendTokenCookie(string cookieName, string token, DateTimeOffset expires)
    {
        Response.Cookies.Append(cookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = expires
        });
    }
}