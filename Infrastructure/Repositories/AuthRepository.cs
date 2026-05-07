
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedViewModel.DTOs;

namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthRepository(UserManager<User> userManager, IConfiguration configuration, AppDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {

            return new AuthResponseDto
            {
                IsSuccess = false,
            };
        }
        var roles = await _userManager.GetRolesAsync(user);

        // var refreshTokenString = GenerateRefreshToken();

        // var refreshToken = new RefreshToken
        // {
        //     Token = refreshTokenString,
        //     Expires = DateTime.UtcNow.AddDays(7),
        //     UserId = user.Id,
        //     IsRevoked = false,
        // };

        // _context.RefreshTokens.Add(refreshToken);
        // await _context.SaveChangesAsync();

        var AccessTokenGen = await GenerateAccessTokenAsync(user);
        return new AuthResponseDto
        {
            IsSuccess = true,
            AccessToken = AccessTokenGen,
            Roles = roles.ToList()
        };
    }
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
            };
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new AuthResponseDto {IsSuccess = false};
        }

        await _userManager.AddToRoleAsync(user, "Customer");

        return new AuthResponseDto { IsSuccess = true};
    }
    //Stateless + Stateful
    private async Task<string> GenerateAccessTokenAsync(User user)
    {
        // This SecurityTokenDescriptor is created just to write the code more clean, don't need to create many constructors for the token.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authorization:Key"]!));
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.GivenName, user.FirstName)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(1),
            Issuer = _configuration["Authorization:Issuer"],
            Audience = _configuration["Authorization:Audience"],
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    // public async Task<AuthResponseDto> RefreshTokenAsync(AuthResponseDto request)
    // {
    //     var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken);
    //     if (existingToken == null || existingToken.IsRevoked == true || existingToken.Expires < DateTime.UtcNow)
    //     {
    //         return new AuthResponseDto
    //         {
    //             IsSuccess = false,
    //             Message = "Token không hợp lệ hoặc đã hết hạn"
    //         };
    //     }
    //     var user = await _userManager.FindByIdAsync(existingToken.UserId);
    //     var newAccessToken = await GenerateAccessTokenAsync(user!);
    //     var newRefreshTokenString = GenerateRefreshToken();
    //     existingToken.IsRevoked = true;
    //     var newRefreshToken = new RefreshToken
    //     {
    //         Token = newRefreshTokenString,
    //         Expires = DateTime.UtcNow.AddDays(7),
    //         UserId = user!.Id,
    //         IsRevoked = false
    //     };
    //     _context.RefreshTokens.Add(newRefreshToken);
    //     await _context.SaveChangesAsync();
    //     return new AuthResponseDto
    //     {
    //         IsSuccess = true,
    //         Message = "Token đã được làm mới",
    //         AccessToken = newAccessToken,
    //         RefreshToken = newRefreshTokenString
    //     };
    // }
    // // Helper
    // private string GenerateRefreshToken()
    // {
    //     var randomNumber = new Byte[32];
    //     using var rng = RandomNumberGenerator.Create();
    //     rng.GetBytes(randomNumber);
    //     return Convert.ToBase64String(randomNumber);
    // }
}