using BussinessLogic.Entities;
using SharedViewModel.DTOs;

namespace BusinessLogic.Mapper
{
    public static class AuthMapper
    {
        public static User ToEntity(RegisterRequestDto request)
        {
            return new User
            {
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                LastName = request.LastName?.Trim() ?? string.Empty,
                Email = request.Email?.Trim().ToLower() ?? string.Empty
            };
        }

        public static AuthResponseDto ToResponseDto(
            bool isSuccess,
            string message,
            List<string> roles,
            string accessToken)
        {
            return new AuthResponseDto
            {
                IsSuccess = isSuccess,
                Message = message,
                Roles = roles ?? new List<string>(),
                AccessToken = accessToken ?? string.Empty
            };
        }

        public static LoginRequestDto ToLoginDto(string email, string password)
        {
            return new LoginRequestDto
            {
                Email = email?.Trim().ToLower() ?? string.Empty,
                Password = password ?? string.Empty
            };
        }
    }
}