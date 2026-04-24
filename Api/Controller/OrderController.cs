using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;

namespace Api.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _IOrderRepository;
        public OrderController(IOrderRepository OrderRepository)
        {
            _IOrderRepository = OrderRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            if (request.Items == null || !request.Items.Any())
            { return BadRequest("Order is Empty or null"); }
            var newOder = new Order
            {
                UserId = request.UserId,
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

            var createdOrderId = await _IOrderRepository.CreateOrderAsync(newOder);
            return Ok(new { OrderId = createdOrderId, Message = "Order created successfully." });

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            // FETCH: Get Entity from Infrastructure
            var orderEntity = await _IOrderRepository.GetOrderByIdAsync(id);

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