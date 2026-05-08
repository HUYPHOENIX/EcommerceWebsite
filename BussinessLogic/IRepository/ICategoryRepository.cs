using BussinessLogic.Entities;
using SharedViewModel.DTOs;

namespace BussinessLogic.IRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllCategories();
        Task<CategoryDto> GetCategorybyID(int id);
        Task<CategoryDto> AddCategory(CategoryDto categoryDto);
        Task UpdateCategory(CategoryDto categoryDto);
        Task DeleteCategory(int id); 
    }
}