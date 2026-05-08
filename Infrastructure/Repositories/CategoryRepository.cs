
using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedViewModel.DTOs;

namespace Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> AddCategory(CategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description
        };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task UpdateCategory(CategoryDto category)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        
        if (category == null)
            throw new KeyNotFoundException();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategories()
    {
        var categories = await _context.Categories.ToListAsync();

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

    }

    public async Task<CategoryDto> GetCategorybyID(int id)
    {
        throw new NotImplementedException();
    }
}