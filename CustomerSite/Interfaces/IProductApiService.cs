using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces
{
    public interface IProductApiService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<PagedItems<ProductDto>?> GetProductsByPageAsync(int? categoryId, int page, int pageSize);
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}
