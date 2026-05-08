using BussinessLogic.Entities;

namespace BussinessLogic.IRepository
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsByPageAsync(int? CategoryId, int PageNumber, int PageSize);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetProductByID(int id);
        Task<List<Product>> GetProductsByID(List<int> ids);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

    }
}