using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces;

public interface IOrderApiClient
{
    Task<int?> CreateOrderAsync(OrderRequestDto request);
}