using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedViewModel.DTOs;

namespace BussinessLogic.Services;
public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(TokenGenerationRequest request);
}

public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateAccessTokenAsync(TokenGenerationRequest request)
        {
            if(request == null)
            throw new ArgumentNullException("Request không được trống.");
            if (request.Email == null || request.FirstName == null || request.Roles == null || request.UserId == null)
                throw new ArgumentNullException("1 trông các field không được trống");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authorization:Key"]!)
            );

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.UserId!),
                new Claim(JwtRegisteredClaimNames.Email, request.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, request.FirstName!)
            };
            foreach (var Role in request.Roles!)
            {
                claims.Add(new Claim(ClaimTypes.Role, Role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),  
                Issuer = _configuration["Authorization:Issuer"],
                Audience = _configuration["Authorization:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }
    }