
using BussinessLogic.IRepository;
using BusinessLogic.Mapper;
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
            var category = CategoryMapper.ToEntity(dto);
            var created = await _repository.AddCategory(category);
            return CategoryMapper.ToDto(created);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _repository.GetCategorybyID(dto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy category id : {dto.Id}");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên category không được để trống.");
            if (dto.Name.Length > 50)
                throw new ArgumentException("Tên category không được quá 50 ký tự.");

            if ( dto.Description.Length > 100)
                throw new ArgumentException("Mô tả category không được quá 100 ký tự.");

            CategoryMapper.UpdateEntity(existing, dto);
            var updated = await _repository.UpdateCategory(existing);
            return CategoryMapper.ToDto(updated);
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

            return CategoryMapper.ToDto(category);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetAllCategories();
            if (categories == null)
                throw new ArgumentException("Không lấy được danh sách");
            return categories.Select(CategoryMapper.ToDto).ToList();
        }

    }
}