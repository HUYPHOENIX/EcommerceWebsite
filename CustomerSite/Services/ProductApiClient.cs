

using CustomerSite.Interfaces;
using SharedViewModel.DTOs;

namespace CustomerSite.Services
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly HttpClient _httpClient;
        public ProductApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/products");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
                return data ?? new List<ProductDto>();
            }
            return new List<ProductDto>();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            return null; // Return null if it fails or 404s
        }
    }
}