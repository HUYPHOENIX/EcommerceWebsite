using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceShop.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    // We inject the database context into the constructor
    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    //
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategorybyID(int id )
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category; // Returns the category with its new SQL ID attached
    }
    
    public async Task UpdateAsync(Category category)
    {
        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

}