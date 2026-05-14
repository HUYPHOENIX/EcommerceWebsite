using System.Runtime.InteropServices;
using BusinessLogic.Mapper;
using BussinessLogic.IRepository;
using SharedViewModel.DTOs;

namespace BussinessLogic.Services
{


    public interface IProductService
    {
        Task<IEnumerable<ProductListDto>> GetAllProductsAsync();
        Task<ProductDetailDto> GetProductByIdAsync(int id);
        Task<PagedItems<ProductListDto>> GetProductsByPageAsync(
            int? categoryId, int pageNumber, int pageSize);
        Task<ProductDetailDto> CreateProductAsync(CreateProductRequest request);
        Task<ProductDetailDto> UpdateProductAsync(int id, UpdateProductRequest request);
        Task DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        private void ValidateProductName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên sản phẩm không được để trống");

            if (name.Length < 3)
                throw new ArgumentException("Tên sản phẩm phải có ít nhất 3 ký tự");

            if (name.Length > 100)
                throw new ArgumentException("Tên sản phẩm không được vượt quá 100 ký tự");
        }

        private void ValidateDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 2000)
                throw new ArgumentException("Mô tả sản phẩm không được vượt quá 2000 ký tự");
        }

        private void ValidatePrice(decimal price)
        {
            if (price <= 0)
                throw new ArgumentException("Giá sản phẩm phải lớn hơn 0");

            if (price > 999999999)
                throw new ArgumentException("Giá sản phẩm không được quá cao");
        }

        private void ValidateSizes(List<string> sizes)
        {
            if (sizes == null || sizes.Count == 0)
                throw new ArgumentException("Sản phẩm phải có ít nhất một size");

            if (sizes.Count > 10)
                throw new ArgumentException("Sản phẩm không được có quá 10 sizes");

            foreach (var size in sizes)
            {
                if (string.IsNullOrWhiteSpace(size))
                    throw new ArgumentException("Size không được để trống");

                if (size.Length > 10)
                    throw new ArgumentException("Tên size không được vượt quá 10 ký tự");
            }
        }

        private void ValidateColors(List<string> colors)
        {
            if (colors == null || colors.Count == 0)
                throw new ArgumentException("Sản phẩm phải có ít nhất một màu");
            if (colors.Count > 50)
                throw new ArgumentException("Sản phẩm không được có quá 50 màu");
            foreach (var color in colors)
            {
                if (string.IsNullOrWhiteSpace(color))
                    throw new ArgumentException("Màu không được để trống");
                if (color.Length > 50)
                    throw new ArgumentException("Tên màu không được vượt quá 50 ký tự");
            }
        }

        private void ValidateCategoryId(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID phải lớn hơn 0");
        }

        private void ValidateImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image Không được trống.");
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Image URL không hợp lệ");
        }

        public async Task<IEnumerable<ProductListDto>> GetAllProductsAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(ProductMapper.ToListDto);
        }

        public async Task<ProductDetailDto> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID phải lớn hơn 0");

            var product = await _repository.GetProductByIDAsync(id);

            if (product == null)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID: {id}");

            return ProductMapper.ToDetailDto(product);
        }

        public async Task<PagedItems<ProductListDto>> GetProductsByPageAsync(
            int? categoryId, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1 || pageSize > 12)
                pageSize = 12;
            var (items, totalCount) = await _repository.GetProductsByPageAsync(
            categoryId, pageNumber, pageSize);
            if (items == null)
            {
                throw new ArgumentNullException("Danh sách bị rỗng khi lấy từ dưới DB lên.");
            }
            return new PagedItems<ProductListDto>
            {
                Items = items.Select(ProductMapper.ToListDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }


        public async Task<ProductDetailDto> CreateProductAsync(CreateProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateProductName(request.Name);
            ValidateDescription(request.Description);
            ValidatePrice(request.Price);
            ValidateSizes(request.Sizes);
            ValidateColors(request.Colors);
            ValidateCategoryId(request.CategoryId);
            ValidateImageUrl(request.ImageUrl);

            var product = ProductMapper.ToEntity(request);

            var created = await _repository.AddProductAsync(product);

            return ProductMapper.ToDetailDto(created);
        }


        public async Task<ProductDetailDto> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (id <= 0)
                throw new ArgumentException("Product ID phải lớn hơn 0");

            if (id != request.Id)
                throw new ArgumentException("Product ID trong URL và body không match");

            var existing = await _repository.GetProductByIDAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID: {id}");

            ValidateProductName(request.Name);
            ValidateDescription(request.Description);
            ValidatePrice(request.Price);
            ValidateSizes(request.Sizes);
            ValidateColors(request.Colors);
            ValidateCategoryId(request.CategoryId);
            ValidateImageUrl(request.ImageUrl);

            ProductMapper.UpdateEntity(existing, request);

            await _repository.UpdateProductAsync(existing);

            return ProductMapper.ToDetailDto(existing);
        }

        public async Task DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID phải lớn hơn 0");

            var product = await _repository.GetProductByIDAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID: {id}");

            await _repository.DeleteProductAsync(id);
        }
    }
}

