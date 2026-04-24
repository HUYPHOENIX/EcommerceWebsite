using CustomerSite.Interfaces;
using SharedViewModel.DTOs;

namespace CustomerSite.Services;

public class OrderApiClient : IOrderApiClient
{
    private readonly HttpClient _httpClient;

    public OrderApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int?> CreateOrderAsync(OrderRequestDto request)
    {
        // 1. Send the POST request to http://localhost:5007/api/order
        var response = await _httpClient.PostAsJsonAsync("/api/order", request);

        // 2. Check if the API returned 200 OK
        if (response.IsSuccessStatusCode)
        {
            // Read the JSON response to extract the new OrderId
            var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            return result?.OrderId; 
        }

        // 3. If it failed (e.g., 400 Bad Request), return null
        return null;
    }
}