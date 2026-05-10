namespace BusinessLogic.Mapper
{
    using BussinessLogic.Entities;
    using SharedViewModel.DTOs;

    public static class ProductMapper
    {
        public static ProductListDto ToListDto(Product product)
        {
            return new ProductListDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };
        }

        public static ProductDetailDto ToDetailDto(Product product)
        {
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Sizes = product.Sizes,
                Colors = product.Colors,
                CategoryId = product.CategoryId,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };
        }

        public static Product ToEntity(CreateProductRequest request)
        {
            return new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                Sizes = request.Sizes ?? new List<string>(),
                Colors = request.Colors ?? new List<string>(),
                CategoryId = request.CategoryId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(Product product, UpdateProductRequest request)
        {
            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim() ?? string.Empty;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.Sizes = request.Sizes ?? new List<string>();
            product.Colors = request.Colors ?? new List<string>();
            product.CategoryId = request.CategoryId;
            product.UpdatedDate = DateTime.UtcNow;
        }
    }
}