// using BussinessLogic.IRepository;
// using BussinessLogic.Entities;
// using SharedViewModel.DTOs;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using System.Text.RegularExpressions;

// namespace Api.Controller
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class CategoriesController : ControllerBase
//     {
//         private readonly ICategoryRepository _categoryRepository;
//         // Dependency Injection hands us the worker we registered in Program.cs!
//         public CategoriesController(ICategoryRepository categoryRepository)
//         {
//             _categoryRepository = categoryRepository;
//         }

//         //Get: api/categories
//         [HttpGet("GetCategories")]
//         public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
//         {
//             try
//             {
//                 var categories = await _categoryRepository.GetAllCategories();
//                 return Ok(categories);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = "Error fetching categories", error = ex.Message });
//             }
//         }

//         [HttpPost("CreateCategory")]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> CreateCategory([FromBody] CategoryDto request)
//         {

//             if (string.IsNullOrWhiteSpace(request.Name))
//                 return BadRequest("Category ít nhất phải có tên");
//             if (string.IsNullOrWhiteSpace(request.Name))
//             {
//                 return BadRequest(new
//                 {
//                     success = false,
//                     message = "Tên danh mục không được để trống."
//                 });
//             }
//             string pattern = @"^(?=.*[a-zA-ZÀ-ỹ])[a-zA-ZÀ-ỹ0-9 ]+$";
//             if (!Regex.IsMatch(request.Name, pattern))
//             {
//                 return BadRequest(new
//                 {
//                     success = false,
//                     message = "Tên danh mục không hợp lệ! Không được chứa ký tự đặc biệt và phải có ít nhất một chữ cái."
//                 });
//             }
//             try
//             {
//                 var result = await _categoryRepository.AddCategory(request);
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = "Lỗi khởi tạo", error = ex.Message });
//             }

//         }

//         // [HttpPut("{id}")]
//         // [Authorize(Roles = "Admin")] // <--- Locks down PUT
//         // public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto request)
//         // {
//         //     if (!ModelState.IsValid) return BadRequest(ModelState);

//         //     var response = await _categoryRepository.UpdateAsync(id, request);
//         //     if (!response.IsSuccess)
//         //     {
//         //         return BadRequest(response);
//         //     }

//         //     return Ok(response);
//         // }

//         [HttpDelete("DeleteCategory/{id}")]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> DeleteCategory(int id)
//         {
//             try
//             {
//                 await _categoryRepository.DeleteCategory(id);
//                 return NoContent();
//             }
//             catch (KeyNotFoundException)
//             {
//                 return NotFound(new { message = "Không có bất kỳ category nào như trên." });
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { message = "Lỗi xóa bỏ.", error = ex.Message });
//             }
//         }

//     }
// }