using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using SharedViewModel.DTOs;
using BusinessLogic.Mapper;
using Microsoft.AspNetCore.Identity;

namespace BussinessLogic.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginCustomerAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAdminAsync(LoginRequestDto request);
    }

    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        public AuthService(ITokenService tokenRepository, UserManager<User> userManager)
        {
            _tokenService = tokenRepository;
            _userManager = userManager;
        }

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request,
            string? requiredRole = null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return AuthMapper.ToResponseDto(
                    false,
                    "Email không được trống",
                    new List<string>(),
                    string.Empty);

            if (string.IsNullOrWhiteSpace(request.Password))
                return AuthMapper.ToResponseDto(
                    false,
                    "Password không được trống",
                    new List<string>(),
                    string.Empty);

            if (!IsValidEmail(request.Email))
                return AuthMapper.ToResponseDto(
                    false,
                    "Email không hợp lệ",
                    new List<string>(),
                    string.Empty);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return AuthMapper.ToResponseDto(
                    false,
                    "Email hoặc password không đúng",
                    new List<string>(),
                    string.Empty);

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
                return AuthMapper.ToResponseDto(
                     false,
                     "Email hoặc password không đúng",
                     new List<string>(),
                     string.Empty);

            var roles = await _userManager.GetRolesAsync(user);

            if (!string.IsNullOrEmpty(requiredRole) && !roles.Contains(requiredRole))
            {
                return AuthMapper.ToResponseDto(
                   false,
                   $"Bạn không có quyền '{requiredRole}'",
                   roles.ToList(),
                   string.Empty);
            }
            var newRequestToken = new TokenGenerationRequest
            {
                Email = user.Email ?? user.NormalizedEmail,
                FirstName = user.FirstName,
                Roles = roles,
                UserId = user.Id
            };
            var token = await _tokenService.GenerateAccessTokenAsync(newRequestToken);

            return AuthMapper.ToResponseDto(
                true,
                "Login thành công",
                roles.ToList(),
                token);
        }

        public async Task<AuthResponseDto> LoginCustomerAsync(LoginRequestDto request)
        {
            return await LoginAsync(request);
        }

        public async Task<AuthResponseDto> LoginAdminAsync(LoginRequestDto request)
        {
            return await LoginAsync(request, "Admin");
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                return AuthMapper.ToResponseDto(
                    false,
                    "Tất cả các field không được để trống",
                    new List<string>(),
                    string.Empty);
            }

            if (!IsValidEmail(request.Email))
                return AuthMapper.ToResponseDto(
                    false,
                    "Email không hợp lệ",
                    new List<string>(),
                    string.Empty);
            if (!IsValidPassword(request.Password))
                return AuthMapper.ToResponseDto(
                    false,
                    "Password phải có ít nhất 8 ký tự, chứa chữ hoa, chữ thường, số",
                    new List<string>(),
                    string.Empty);
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return AuthMapper.ToResponseDto(
                    false,
                    "Email đã được sử dụng",
                    new List<string>(),
                    string.Empty);
            var user = AuthMapper.ToEntity(request);
            user.UserName = request.Email;
            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                return AuthMapper.ToResponseDto(
                    false,
                    $"Lỗi đăng ký: {string.Join(", ", createResult.Errors.Select(e => e.Description))}",
                    new List<string>(),
                    string.Empty);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return AuthMapper.ToResponseDto(
                true,
                "Đăng ký thành công",
                new List<string> { "Customer" },
                string.Empty);
        }

        // Helper methods
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpper && hasLower && hasDigit;
        }
    }
}