using BussinessLogic.Entities;
using SharedViewModel.DTOs;

namespace BusinessLogic.Mapper
{


    public static class CategoryMapper
    {
        public static CategoryDto ToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public static Category ToEntity(CategoryDto request)
        {
            return new Category
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty
            };
        }

        public static void UpdateEntity(Category category, CategoryDto request)
        {
            category.Name = request.Name.Trim();
            category.Description = request.Description?.Trim() ?? string.Empty;
        }
    }
}