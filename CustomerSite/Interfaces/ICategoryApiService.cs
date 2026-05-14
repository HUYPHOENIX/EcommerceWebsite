using SharedViewModel.DTOs;

namespace CustomerSite.Interfaces;
public interface ICategoryApiService
{
    Task<List<CategoryDto>?> GetCate ();
}