using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces
{
    public interface IProductApiClient
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}
