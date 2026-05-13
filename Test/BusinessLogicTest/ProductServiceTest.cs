// Tests/BussinessLogic.Tests/Services/ProductServiceTests.cs
using Xunit;
using Moq;
using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using BussinessLogic.Services;
using BusinessLogic.Mapper;
using SharedViewModel.DTOs;

namespace BussinessLogic.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepository.Object);
        }

        #region GetAllProductsAsync Tests

        [Fact]
        public async Task GetAllProductsAsync_WithProducts_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product 
                { 
                    Id = 1, 
                    Name = "Product 1", 
                    Price = 100000,
                    CategoryId = 1,
                    ImageUrl = "https://example.com/img1.jpg",
                    Sizes = new List<string> { "S", "M" },
                    Colors = new List<string> { "Red" }
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "Product 2", 
                    Price = 200000,
                    CategoryId = 1,
                    ImageUrl = "https://example.com/img2.jpg",
                    Sizes = new List<string> { "L" },
                    Colors = new List<string> { "Blue" }
                }
            };

            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetProductByIdAsync Tests

        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 150000,
                Description = "Test description",
                CategoryId = 1,
                ImageUrl = "https://example.com/image.jpg",
                Sizes = new List<string> { "M", "L" },
                Colors = new List<string> { "Red", "Blue" }
            };

            _mockRepository
                .Setup(r => r.GetProductByIDAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Name);
            Assert.Equal(150000, result.Price);
            _mockRepository.Verify(r => r.GetProductByIDAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetProductByIdAsync(0)
            );
            Assert.Contains("Product ID phải lớn hơn 0", ex.Message);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetProductByIDAsync(999))
                .ReturnsAsync(default(Product));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetProductByIdAsync(999)
            );
            Assert.Contains("999", ex.Message);
        }

        #endregion

        #region GetProductsByPageAsync Tests

        [Fact]
        public async Task GetProductsByPageAsync_WithValidParams_ReturnsPaginatedProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product 
                { 
                    Id = 1, 
                    Name = "Product 1", 
                    Price = 100000,
                    CategoryId = 1,
                    ImageUrl = "https://example.com/img1.jpg",
                    Sizes = new List<string> { "S" },
                    Colors = new List<string> { "Red" }
                }
            };

            _mockRepository
                .Setup(r => r.GetProductsByPageAsync(null, 1, 12))
                .ReturnsAsync((products, 1));

            // Act
            var result = await _service.GetProductsByPageAsync(null, 1, 12);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.PageNumber);
            _mockRepository.Verify(r => r.GetProductsByPageAsync(null, 1, 12), Times.Once);
        }

        [Fact]
        public async Task GetProductsByPageAsync_WithCategoryFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product 
                { 
                    Id = 1, 
                    Name = "Electronics", 
                    CategoryId = 1,
                    Price = 500000,
                    ImageUrl = "https://example.com/img.jpg",
                    Sizes = new List<string> { "Standard" },
                    Colors = new List<string> { "Black" }
                }
            };

            _mockRepository
                .Setup(r => r.GetProductsByPageAsync(1, 1, 12))
                .ReturnsAsync((products, 5));

            // Act
            var result = await _service.GetProductsByPageAsync(1, 1, 12);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(5, result.TotalCount);
        }

        [Fact]
        public async Task GetProductsByPageAsync_WithInvalidPageNumber_AutoCorrects()
        {
            // Arrange
            var products = new List<Product>();
            _mockRepository
                .Setup(r => r.GetProductsByPageAsync(null, 1, 12))
                .ReturnsAsync((products, 0));

            // Act
            var result = await _service.GetProductsByPageAsync(null, 0, 12);

            // Assert
            Assert.Equal(1, result.PageNumber); // Auto corrected to 1
        }

        [Fact]
        public async Task GetProductsByPageAsync_WithInvalidPageSize_AutoCorrects()
        {
            // Arrange
            var products = new List<Product>();
            _mockRepository
                .Setup(r => r.GetProductsByPageAsync(null, 1, 12))
                .ReturnsAsync((products, 0));

            // Act
            var result = await _service.GetProductsByPageAsync(null, 1, 100);

            // Assert
            Assert.Equal(12, result.PageSize); // Auto corrected to 12
        }

        #endregion

        #region CreateProductAsync Tests

        [Fact]
        public async Task CreateProductAsync_WithValidData_ReturnsCreatedProduct()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "New Product",
                Price = 250000,
                Description = "New product description",
                CategoryId = 1,
                ImageUrl = "https://example.com/new-product.jpg",
                Sizes = new List<string> { "S", "M", "L" },
                Colors = new List<string> { "Red", "Blue" }
            };

            var product = new Product
            {
                Id = 1,
                Name = "New Product",
                Price = 250000,
                CategoryId = 1,
                ImageUrl = "https://example.com/new-product.jpg",
                Sizes = new List<string> { "S", "M", "L" },
                Colors = new List<string> { "Red", "Blue" }
            };

            _mockRepository
                .Setup(r => r.AddProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(product);

            // Act
            var result = await _service.CreateProductAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.Equal(250000, result.Price);
            _mockRepository.Verify(r => r.AddProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.CreateProductAsync(null)
            );
        }

        [Fact]
        public async Task CreateProductAsync_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "",
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("không được để trống", ex.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithShortName_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "AB",
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("ít nhất 3 ký tự", ex.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithLongName_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = new string('a', 101),
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("100 ký tự", ex.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidPrice_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Product",
                Price = 0,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("lớn hơn 0", ex.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithoutSizes_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Product",
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string>(),
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("ít nhất một size", ex.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithoutColors_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Product",
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string>()
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("ít nhất một màu", ex.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidCategoryId_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Product",
                Price = 100000,
                CategoryId = 0,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("lớn hơn 0", ex.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidImageUrl_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Product",
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "invalid-url",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateProductAsync(request)
            );
            Assert.Contains("không hợp lệ", ex.Message);
        }

        #endregion

        #region UpdateProductAsync Tests

        [Fact]
        public async Task UpdateProductAsync_WithValidData_ReturnsUpdatedProduct()
        {
            // Arrange
            var request = new UpdateProductRequest
            {
                Id = 1,
                Name = "Updated Product",
                Price = 300000,
                Description = "Updated description",
                CategoryId = 1,
                ImageUrl = "https://example.com/updated.jpg",
                Sizes = new List<string> { "M", "L" },
                Colors = new List<string> { "Green" }
            };

            var existingProduct = new Product
            {
                Id = 1,
                Name = "Old Product",
                Price = 200000,
                CategoryId = 1,
                ImageUrl = "https://example.com/old.jpg",
                Sizes = new List<string> { "S" },
                Colors = new List<string> { "Red" }
            };

            var updatedProduct = new Product
            {
                Id = 1,
                Name = "Updated Product",
                Price = 300000,
                CategoryId = 1,
                ImageUrl = "https://example.com/updated.jpg",
                Sizes = new List<string> { "M", "L" },
                Colors = new List<string> { "Green" }
            };

            _mockRepository
                .Setup(r => r.GetProductByIDAsync(1))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _service.UpdateProductAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Product", result.Name);
            Assert.Equal(300000, result.Price);
            _mockRepository.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.UpdateProductAsync(1, null)
            );
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidId_ThrowsArgumentException()
        {
            // Arrange
            var request = new UpdateProductRequest { Id = 1 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateProductAsync(0, request)
            );
            Assert.Contains("lớn hơn 0", ex.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WithMismatchedId_ThrowsArgumentException()
        {
            // Arrange
            var request = new UpdateProductRequest
            {
                Id = 1,
                Name = "Product",
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateProductAsync(2, request)
            );
            Assert.Contains("không match", ex.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var request = new UpdateProductRequest
            {
                Id = 999,
                Name = "Product",
                Price = 100000,
                CategoryId = 1,
                ImageUrl = "https://example.com/img.jpg",
                Sizes = new List<string> { "M" },
                Colors = new List<string> { "Red" }
            };

            _mockRepository
                .Setup(r => r.GetProductByIDAsync(999))
                .ReturnsAsync(default(Product));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateProductAsync(999, request)
            );
            Assert.Contains("999", ex.Message);
        }

        #endregion

        #region DeleteProductAsync Tests

        [Fact]
        public async Task DeleteProductAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product" };

            _mockRepository
                .Setup(r => r.GetProductByIDAsync(1))
                .ReturnsAsync(product);

            _mockRepository
                .Setup(r => r.DeleteProductAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteProductAsync(1);

            // Assert
            _mockRepository.Verify(r => r.DeleteProductAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_WithInvalidId_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.DeleteProductAsync(0)
            );
            Assert.Contains("lớn hơn 0", ex.Message);
        }

        [Fact]
        public async Task DeleteProductAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetProductByIDAsync(999))
                .ReturnsAsync(default(Product));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.DeleteProductAsync(999)
            );
            Assert.Contains("999", ex.Message);
        }

        #endregion
    }
}