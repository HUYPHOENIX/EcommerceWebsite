using CustomerSite.Interfaces;
using SharedViewModel.DTOs;

namespace CustomerSite.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;

    public AccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Auth/Login", request);
        if (response.IsSuccessStatusCode || response!= null)
        {
           return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }
        return null;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", request);
        if (response.IsSuccessStatusCode || response!= null)
        {
            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }
        return null;
    }
}