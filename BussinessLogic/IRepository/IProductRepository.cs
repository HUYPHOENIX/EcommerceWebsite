using BussinessLogic.Entities;

namespace BussinessLogic.IRepository
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsByPageAsync(int? CategoryId, int PageNumber, int PageSize);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetProductByIDAsync(int id);
        Task<List<Product>> GetProductsByIDAsync(List<int> ids);
        Task<Product> AddProductAsync(Product product);
        Task<Product>UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}