using CustomerSite.Interfaces;
using SharedViewModel.DTOs;

namespace CustomerSite.Services
{
    public class CategoryService : ICategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryDto>?> GetCate()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/categories/GetAll");

                if (!response.IsSuccessStatusCode)
                    return new List<CategoryDto>();

                return await response.Content
                    .ReadFromJsonAsync<List<CategoryDto>>()
                    ?? new List<CategoryDto>();
            }
            catch (HttpRequestException) 
            {
                return null;
            }
        }
    }
}