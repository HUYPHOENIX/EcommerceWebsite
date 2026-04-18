using BussinessLogic.Interfaces;
using BussinessLogic.Entities;
using SharedViewModel.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        // Dependency Injection hands us the worker we registered in Program.cs!
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        //Get: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {

            var categories = await _categoryRepository.GetAllAsync();
            // Map the entities to DTOs
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
            return Ok(categoryDtos);
        }

        //POST : api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto categoryDto)
        {
            // 1. Map the incoming DTO into a real Entity for the database
            var newCategory = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            // 2. Ask the worker to save it
            var createdCategory = await _categoryRepository.AddAsync(newCategory);

            // 3. Attach the brand-new database ID to the DTO to send back to the user
            categoryDto.Id = createdCategory.Id;

            // 4. Return a 201 Created status
            return CreatedAtAction(nameof(GetCategories), new { id = categoryDto.Id }, categoryDto);
        }

    }
}