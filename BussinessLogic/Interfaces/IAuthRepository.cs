using SharedViewModel.DTOs;

namespace BussinessLogic.Interfaces;

public interface IAuthRepository
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(TokenRequestDto request);
}