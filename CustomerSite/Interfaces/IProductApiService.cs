using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces
{
    public interface IProductApiService
    {
        Task<PagedItems<ProductListDto>?> GetProductsByPageAsync(int? categoryId, int page, int pageSize);
        Task<ProductDetailDto?> GetProductByIdAsync(int id);
    }
}
