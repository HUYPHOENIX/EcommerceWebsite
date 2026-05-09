using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace BussinessLogic.Services;
public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);
}

public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;

        public TokenService(IConfiguration configuration, IAuthRepository authRepository)
        {
            _configuration = configuration;
            _authRepository = authRepository;
        }

        public async Task<string> GenerateAccessTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authorization:Key"]!)
            );

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName)
            };
            var roles = await _authRepository.GetRolesAsync(user);
            foreach (var role in roles.ToList())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
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