using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using SharedViewModel.DTOs;

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
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        public AuthService(IAuthRepository authRepository, ITokenService tokenRepository)
        {
            _authRepository = authRepository;
            _tokenService = tokenRepository;
        }

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request,
            string? requiredRole = null) 
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email không được trống"
                };

            if (string.IsNullOrWhiteSpace(request.Password))
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Password không được trống"
                };

            if (!IsValidEmail(request.Email))
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email không hợp lệ"
                };

            var user = await _authRepository.FindByEmailAsync(request.Email);
            if (user == null)
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email hoặc password không đúng"
                };

            var passwordValid = await _authRepository.ValidatePasswordAsync(user, request.Password);
            if (!passwordValid)
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email hoặc password không đúng"
                };

            var roles = await _authRepository.GetRolesAsync(user);

            if (!string.IsNullOrEmpty(requiredRole) && !roles.Contains(requiredRole))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Bạn không có quyền '{requiredRole}'"
                };
            }

            var token = await _tokenService.GenerateAccessTokenAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login thành công",
                AccessToken = token,
                Roles = roles.ToList()
            };
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
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Tất cả các field không được để trống",
                    AccessToken = string.Empty,
                };
            }

            if (!IsValidEmail(request.Email))
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email không hợp lệ,"

                };

            if (!IsValidPassword(request.Password))
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Password phải có ít nhất 8 ký tự, chứa chữ hoa, chữ thường, số"
                };

            var existingUser = await _authRepository.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email đã được sử dụng"
                };
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            try
            {
                user = await _authRepository.CreateUserAsync(user, request.Password);
                await _authRepository.AddToRoleAsync(user, "Customer");
                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công",
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Lỗi đăng ký: {ex.Message}"
                };

            }
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