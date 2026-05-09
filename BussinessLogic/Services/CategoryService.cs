using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using SharedViewModel.DTOs;

namespace BussinessLogic.Services
{
    public interface ICategoryService
    {

        Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
        Task<CategoryDto> UpdateCategoryAsync(CategoryDto categoryDto);
        Task<CategoryDto> GetCategoryAsync(int id);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }


    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (dto.Name.Length > 50 || string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên category không được để trống hoặc quá 50 ký tự. ");
            if (dto.Description.Length > 200)
                throw new ArgumentException("Mô tả category không được quá 200 ký tự. ");
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };
            var created = await _repository.AddCategory(category);
            return MapToDto(created);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            var existing = await _repository.GetCategorybyID(dto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy category id : {dto.Id}");
            if (dto.Name.Length > 50 || string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên category không được để trống hoặc quá 50 ký tự. ");
            if (dto.Description.Length > 200)
                throw new ArgumentException("Mô tả category không được quá 200 ký tự. ");
            existing.Name = dto.Name;
            existing.Description = dto.Description;

            var updated = await _repository.UpdateCategory(existing);

            return MapToDto(updated);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var existing = await _repository.GetCategorybyID(id);
            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy category id : {id}");
            await _repository.DeleteCategory(existing);
        }

        public async Task<CategoryDto> GetCategoryAsync(int id)
        {
            var category = await _repository.GetCategorybyID(id);

            if (category == null)
                throw new KeyNotFoundException($"Không tìm thấy category id : {id}");

            return MapToDto(category);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetAllCategories();
            if (categories == null)
                throw new ArgumentException("Không lấy được danh sách");
            return categories.Select(MapToDto).ToList();
        }

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}