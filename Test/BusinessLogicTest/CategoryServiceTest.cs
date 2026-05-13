using Moq;
using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using BussinessLogic.Services;
using SharedViewModel.DTOs;

namespace BussinessLogic.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockRepository;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockRepository = new Mock<ICategoryRepository>();
            _service = new CategoryService(_mockRepository.Object);
        }

        #region CreateCategoryAsync Tests

        [Fact]
        public async Task CreateCategoryAsync_WithValidData_ReturnsCreatedCategory()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Name = "Electronics",
                Description = "Electronic devices"
            };

            var category = new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices"
            };

            _mockRepository
                .Setup(r => r.AddCategory(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            var result = await _service.CreateCategoryAsync(categoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Electronics", result.Name);
            Assert.Equal("Electronic devices", result.Description);
            _mockRepository.Verify(r => r.AddCategory(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithNullDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.CreateCategoryAsync(null)
            );
        }

        [Fact]
        public async Task CreateCategoryAsync_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Name = "",
                Description = "Description"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateCategoryAsync(categoryDto)
            );
            Assert.Contains("không được để trống", ex.Message);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithNameExceeding50Chars_ThrowsArgumentException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Name = new string('a', 51),
                Description = "Description"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateCategoryAsync(categoryDto)
            );
            Assert.Contains("quá 50 ký tự", ex.Message);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithDescriptionExceeding200Chars_ThrowsArgumentException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Name = "Valid Name",
                Description = new string('a', 201)
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateCategoryAsync(categoryDto)
            );
            Assert.Contains("quá 200 ký tự", ex.Message);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithWhitespaceOnlyName_ThrowsArgumentException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Name = "   ",
                Description = "Description"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateCategoryAsync(categoryDto)
            );
        }

        #endregion

        #region UpdateCategoryAsync Tests

        [Fact]
        public async Task UpdateCategoryAsync_WithValidData_ReturnsUpdatedCategory()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description"
            };

            var existingCategory = new Category
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description"
            };

            var updatedCategory = new Category
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description"
            };

            _mockRepository
                .Setup(r => r.GetCategorybyID(1))
                .ReturnsAsync(existingCategory);

            _mockRepository
                .Setup(r => r.UpdateCategory(It.IsAny<Category>()))
                .ReturnsAsync(updatedCategory);

            // Act
            var result = await _service.UpdateCategoryAsync(categoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("Updated Description", result.Description);
            _mockRepository.Verify(r => r.UpdateCategory(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithNullDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.UpdateCategoryAsync(null)
            );
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Id = 999,
                Name = "Name",
                Description = "Description"
            };

            _mockRepository
                .Setup(r => r.GetCategorybyID(999))
                .ReturnsAsync((Category)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateCategoryAsync(categoryDto)
            );
            Assert.Contains("999", ex.Message);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Id = 1,
                Name = "",
                Description = "Description"
            };

            var existingCategory = new Category { Id = 1 };

            _mockRepository
                .Setup(r => r.GetCategorybyID(1))
                .ReturnsAsync(existingCategory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateCategoryAsync(categoryDto)
            );
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithNameExceeding50Chars_ThrowsArgumentException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Id = 1,
                Name = new string('a', 51),
                Description = "Description"
            };

            var existingCategory = new Category { Id = 1 };

            _mockRepository
                .Setup(r => r.GetCategorybyID(1))
                .ReturnsAsync(existingCategory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateCategoryAsync(categoryDto)
            );
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithDescriptionExceeding100Chars_ThrowsArgumentException()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Id = 1,
                Name = "Valid Name",
                Description = new string('a', 101)
            };

            var existingCategory = new Category { Id = 1 };

            _mockRepository
                .Setup(r => r.GetCategorybyID(1))
                .ReturnsAsync(existingCategory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateCategoryAsync(categoryDto)
            );
        }

        #endregion

        #region DeleteCategoryAsync Tests

        [Fact]
        public async Task DeleteCategoryAsync_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test" };

            _mockRepository
                .Setup(r => r.GetCategorybyID(1))
                .ReturnsAsync(category);

            _mockRepository
                .Setup(r => r.DeleteCategory(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteCategoryAsync(1);

            // Assert
            _mockRepository.Verify(r => r.DeleteCategory(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetCategorybyID(999))
                .ReturnsAsync((Category)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.DeleteCategoryAsync(999)
            );
            Assert.Contains("999", ex.Message);
        }

        #endregion

        #region GetCategoryAsync Tests

        [Fact]
        public async Task GetCategoryAsync_WithValidId_ReturnsCategory()
        {
            // Arrange
            var category = new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices"
            };

            _mockRepository
                .Setup(r => r.GetCategorybyID(1))
                .ReturnsAsync(category);

            // Act
            var result = await _service.GetCategoryAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Electronics", result.Name);
            Assert.Equal("Electronic devices", result.Description);
        }

        [Fact]
        public async Task GetCategoryAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetCategorybyID(999))
                .ReturnsAsync((Category)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetCategoryAsync(999)
            );
            Assert.Contains("999", ex.Message);
        }

        #endregion

        #region GetAllCategoriesAsync Tests

        [Fact]
        public async Task GetAllCategoriesAsync_WithCategories_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics", Description = "Devices" },
                new Category { Id = 2, Name = "Clothing", Description = "Clothes" },
                new Category { Id = 3, Name = "Food", Description = "Groceries" }
            };

            _mockRepository
                .Setup(r => r.GetAllCategories())
                .ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Electronics", result[0].Name);
            Assert.Equal("Clothing", result[1].Name);
            Assert.Equal("Food", result[2].Name);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var categories = new List<Category>();

            _mockRepository
                .Setup(r => r.GetAllCategories())
                .ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}