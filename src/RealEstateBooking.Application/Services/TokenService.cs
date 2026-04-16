using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(configuration["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(int userId) => new()
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false,
        CreatedAt = DateTime.UtcNow,
        UserId = userId
    };
}