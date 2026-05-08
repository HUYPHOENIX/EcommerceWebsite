using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsByPageAsync(int? CategoryId, int PageNumber, int PageSize)
        {
            var query = _context.Products.AsQueryable();
            if (CategoryId.HasValue && CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == CategoryId.Value);
            }
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .OrderBy(p => p.Id)
                .ToListAsync();
            return (items, totalItems);
        }

        public async Task<Product?> GetProductByID(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>> GetProductsByID(List<int> ids)
        {
            return await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
    }
}