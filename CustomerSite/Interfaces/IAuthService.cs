using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces;
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
}