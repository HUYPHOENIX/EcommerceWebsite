using CustomerSite.Interfaces;
using SharedViewModel.DTOs;

namespace CustomerSite.Services;

public class OrderApiService: IOrderApiService
{
    private readonly HttpClient _httpClient;

    public OrderApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int?> CreateOrderAsync(OrderRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/order", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            return result?.OrderId; 
        }
        return null;
    }
}