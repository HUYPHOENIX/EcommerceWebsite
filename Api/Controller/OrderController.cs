using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;
using Microsoft.AspNetCore.Authorization;
using BussinessLogic.Services;
using System.Security.Claims;

namespace Api.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserIdFromToken()
        {
            return User.FindFirst("sub")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Không tìm thấy user.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserIdFromToken();

                var result = await _orderService.CreateOrderAsync(userId, request);

                return StatusCode(201, new
                {
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi tạo đơn hàng", error = ex.Message });
            }
        }
    }
}