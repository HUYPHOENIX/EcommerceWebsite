using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces;
public interface IAccountService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
}