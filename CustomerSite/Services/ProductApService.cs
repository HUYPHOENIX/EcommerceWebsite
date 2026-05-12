

using CustomerSite.Interfaces;
using SharedViewModel.DTOs;

namespace CustomerSite.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;
        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<PagedItems<ProductListDto>?> GetProductsByPageAsync(int? categoryId, int page, int pageSize)
        {
            var url = $"/api/products/Paged?page={page}&pageSize={pageSize}";

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                url += $"&categoryId={categoryId.Value}";
            }
            return await _httpClient.GetFromJsonAsync<PagedItems<ProductListDto>>(url);
        }

        public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductDetailDto>();
            }
            return null;
        }
    }
}