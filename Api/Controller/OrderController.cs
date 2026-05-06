using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository OrderRepository)
        {
            _orderRepository = OrderRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            if (request?.Items == null || !request.Items.Any())
            { 
                return BadRequest("Đơn hàng không có gì hết."); 
            }
            var clams = User.Claims.ToList();

            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không xác định được danh tính người dùng.");
            }

            var newOrder = new Order
            {
                UserId = userId!,
                OrderDate = DateTime.UtcNow,
                TotalPrice = request.Items.Sum(x => x.Price * x.Quantity),
                OrderItems = request.Items.Select(dto => new OrderItem
                {
                    ProductId = dto.ProductId,
                    ProductName = dto.ProductName,
                    Price = dto.Price,
                    Size = dto.Size,
                    Color = dto.Color,
                    Quantity = dto.Quantity
                }).ToList()
            };

            var createdOrderId = await _orderRepository.CreateOrderAsync(newOrder);
            return Ok(new { OrderId = createdOrderId, Message = "Đơn hàng được tạo thành công." });

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int id)
        {
            // FETCH: Get Entity from Infrastructure
            var orderEntity = await _orderRepository.GetOrderByIdAsync(id);

            if (orderEntity == null) return NotFound($"Order {id} not found.");

            // TRANSLATE: Entity ➔ DTO
            var responseDto = new OrderResponseDto
            {
                OrderId = orderEntity.Id,
                UserId = orderEntity.UserId,
                OrderDate = orderEntity.OrderDate,
                TotalPrice = orderEntity.TotalPrice,

                Items = orderEntity.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Size = item.Size,
                    Color = item.Color,
                    Quantity = item.Quantity
                }).ToList()
            };
            return Ok(responseDto);
        }
    }
}