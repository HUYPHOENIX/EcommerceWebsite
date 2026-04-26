using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces;

public interface IOrderApiService
{
    Task<int?> CreateOrderAsync(OrderRequestDto request);
}