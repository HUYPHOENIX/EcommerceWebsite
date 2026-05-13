// Tests/BussinessLogic.Tests/Services/OrderServiceTests.cs
using Xunit;
using Moq;
using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using BussinessLogic.Services;
using SharedViewModel.DTOs;

namespace BussinessLogic.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _service = new OrderService(_mockOrderRepository.Object, _mockProductRepository.Object);
        }

        #region CreateOrderAsync - Valid Cases Tests

        [Fact]
        public async Task CreateOrderAsync_WithValidData_ReturnsSuccessOrderResponse()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 2 },
                    new OrderItemDto { ProductId = 2, Quantity = 1 }
                }
            };

            var products = new List<Product>
            {
                new Product 
                { 
                    Id = 1, 
                    Name = "Product 1", 
                    Price = 100000,
                    CategoryId = 1,
                    ImageUrl = "https://example.com/img1.jpg",
                    Sizes = new List<string> { "M" },
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

            var createdOrder = new Order
            {
                Id = 1,
                UserId = userId,
                TotalPrice = 400000, // (100000 * 2) + (200000 * 1)
                OrderItems = new List<OrderItem>()
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(products);

            _mockOrderRepository
                .Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _service.CreateOrderAsync(userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400000, result.TotalPrice);
            _mockOrderRepository.Verify(r => r.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
            _mockProductRepository.Verify(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_WithSingleItem_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = "user-456";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 3 }
                }
            };

            var products = new List<Product>
            {
                new Product 
                { 
                    Id = 1, 
                    Name = "Single Product", 
                    Price = 50000,
                    CategoryId = 1,
                    ImageUrl = "https://example.com/img.jpg",
                    Sizes = new List<string> { "S" },
                    Colors = new List<string> { "Black" }
                }
            };

            var createdOrder = new Order
            {
                Id = 1,
                UserId = userId,
                TotalPrice = 150000, // 50000 * 3
                OrderItems = new List<OrderItem>()
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(products);

            _mockOrderRepository
                .Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _service.CreateOrderAsync(userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(150000, result.TotalPrice);
        }

        [Fact]
        public async Task CreateOrderAsync_WithMultipleQuantities_CalculatesTotalPriceCorrectly()
        {
            // Arrange
            var userId = "user-789";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 2 },
                    new OrderItemDto { ProductId = 2, Quantity = 3 },
                    new OrderItemDto { ProductId = 3, Quantity = 1 }
                }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "P1", Price = 100000, CategoryId = 1, ImageUrl = "https://example.com/1.jpg", Sizes = new List<string> { "M" }, Colors = new List<string> { "Red" } },
                new Product { Id = 2, Name = "P2", Price = 200000, CategoryId = 1, ImageUrl = "https://example.com/2.jpg", Sizes = new List<string> { "L" }, Colors = new List<string> { "Blue" } },
                new Product { Id = 3, Name = "P3", Price = 150000, CategoryId = 1, ImageUrl = "https://example.com/3.jpg", Sizes = new List<string> { "S" }, Colors = new List<string> { "Green" } }
            };

            var expectedTotal = (100000 * 2) + (200000 * 3) + (150000 * 1); // 950000

            var createdOrder = new Order
            {
                Id = 1,
                UserId = userId,
                TotalPrice = expectedTotal,
                OrderItems = new List<OrderItem>()
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(products);

            _mockOrderRepository
                .Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _service.CreateOrderAsync(userId, request);

            // Assert
            Assert.Equal(expectedTotal, result.TotalPrice);
        }

        #endregion

        #region CreateOrderAsync - Validation Error Tests

        [Fact]
        public async Task CreateOrderAsync_WithNullItems_ThrowsArgumentException()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto { Items = null };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(userId, request)
            );
            Assert.Contains("Đơn hàng không có gì hết", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithEmptyItems_ThrowsArgumentException()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto { Items = new List<OrderItemDto>() };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(userId, request)
            );
            Assert.Contains("Đơn hàng không có gì hết", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithNullRequest_ThrowsArgumentException()
        {
            // Arrange
            var userId = "user-123";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(userId, null)
            );
            Assert.Contains("Đơn hàng không có gì hết", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithEmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 1 }
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync("", request)
            );
            Assert.Contains("Không xác định được danh tính người dùng", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithNullUserId_ThrowsArgumentException()
        {
            // Arrange
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 1 }
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(null, request)
            );
            Assert.Contains("Không xác định được danh tính người dùng", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithNonExistentProduct_ThrowsArgumentException()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 999, Quantity = 1 }
                }
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(new List<int> { 999 }))
                .ReturnsAsync(new List<Product>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(userId, request)
            );
            Assert.Contains("Sản phẩm không tồn tại: 999", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithPartialNonExistentProducts_ThrowsArgumentException()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 1 },
                    new OrderItemDto { ProductId = 2, Quantity = 1 },
                    new OrderItemDto { ProductId = 999, Quantity = 1 }
                }
            };

            var existingProducts = new List<Product>
            {
                new Product { Id = 1, Name = "P1", Price = 100000, CategoryId = 1, ImageUrl = "https://example.com/1.jpg", Sizes = new List<string> { "M" }, Colors = new List<string> { "Red" } },
                new Product { Id = 2, Name = "P2", Price = 200000, CategoryId = 1, ImageUrl = "https://example.com/2.jpg", Sizes = new List<string> { "L" }, Colors = new List<string> { "Blue" } }
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(existingProducts);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(userId, request)
            );
            Assert.Contains("Sản phẩm không tồn tại: 999", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithZeroQuantity_ThrowsArgumentException()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 0 }
                }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100000, CategoryId = 1, ImageUrl = "https://example.com/img.jpg", Sizes = new List<string> { "M" }, Colors = new List<string> { "Red" } }
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(products);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(userId, request)
            );
            Assert.Contains("Số lượng không hợp lệ", ex.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_WithNegativeQuantity_ThrowsArgumentException()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = -5 }
                }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100000, CategoryId = 1, ImageUrl = "https://example.com/img.jpg", Sizes = new List<string> { "M" }, Colors = new List<string> { "Red" } }
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(products);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(userId, request)
            );
            Assert.Contains("Số lượng không hợp lệ", ex.Message);
        }

        #endregion

        #region CreateOrderAsync - Edge Cases Tests

        [Fact]
        public async Task CreateOrderAsync_WithDuplicateProductIds_OnlyFetchOnce()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 2 },
                    new OrderItemDto { ProductId = 1, Quantity = 3 }
                }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100000, CategoryId = 1, ImageUrl = "https://example.com/img.jpg", Sizes = new List<string> { "M" }, Colors = new List<string> { "Red" } }
            };

            var createdOrder = new Order
            {
                Id = 1,
                UserId = userId,
                TotalPrice = 500000, // (100000 * 2) + (100000 * 3)
                OrderItems = new List<OrderItem>()
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(products);

            _mockOrderRepository
                .Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _service.CreateOrderAsync(userId, request);

            // Assert
            Assert.Equal(500000, result.TotalPrice);
            _mockProductRepository.Verify(
                r => r.GetProductsByIDAsync(It.Is<List<int>>(list => list.Count == 1)), 
                Times.Once
            );
        }

        [Fact]
        public async Task CreateOrderAsync_WithLargeQuantity_CalculatesCorrectly()
        {
            // Arrange
            var userId = "user-123";
            var request = new OrderRequestDto
            {
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 1000 }
                }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100000, CategoryId = 1, ImageUrl = "https://example.com/img.jpg", Sizes = new List<string> { "M" }, Colors = new List<string> { "Red" } }
            };

            var expectedTotal = 100000 * 1000; // 100000000

            var createdOrder = new Order
            {
                Id = 1,
                UserId = userId,
                TotalPrice = expectedTotal,
                OrderItems = new List<OrderItem>()
            };

            _mockProductRepository
                .Setup(r => r.GetProductsByIDAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(products);

            _mockOrderRepository
                .Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _service.CreateOrderAsync(userId, request);

            // Assert
            Assert.Equal(expectedTotal, result.TotalPrice);
        }

        #endregion
    }
}