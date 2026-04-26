

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
        public async Task<PagedItems<ProductDto>?> GetProductsByPageAsync(int? categoryId, int page, int pageSize)
        {
            var url = $"/api/products?page={page}&pageSize={pageSize}";

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                url += $"&categoryId={categoryId.Value}";
            }
            return await _httpClient.GetFromJsonAsync<PagedItems<ProductDto>>(url);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            return null;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/products/all");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
            return items ?? new List<ProductDto>();
        }
    }
}