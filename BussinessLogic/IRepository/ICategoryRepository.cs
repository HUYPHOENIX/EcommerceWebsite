using BussinessLogic.Entities;
using SharedViewModel.DTOs;

namespace BussinessLogic.IRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategorybyID(int id);
        Task<Category> AddCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task DeleteCategory(Category category); 
    }
}