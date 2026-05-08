using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using Microsoft.AspNetCore.Http.Features;
using SharedViewModel.DTOs;

namespace BussinessLogic.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(string userId, OrderRequestDto request);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(string userId, OrderRequestDto request)
        {
            if (request?.Items == null || !request.Items.Any())
                throw new ArgumentException("Đơn hàng không có gì hết.");

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Không xác định được danh tính người dùng.");

            var productIds = request.Items
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

            var existingProducts = await _productRepository.GetProductsByID(productIds);

            if (existingProducts.Count != productIds.Count)
            {
                var foundIds = existingProducts.Select(p => p.Id).ToList();
                var missingIds = productIds.Except(foundIds).ToList();
                throw new ArgumentException(
                    $"Sản phẩm không tồn tại: {string.Join(", ", missingIds)}");
            }

            // Thêm dictionary "Mục luc" ra là bụp liền không cần duyệt lại từ đầu đến cuối của existingProducts
            var productDict = existingProducts.ToDictionary(p => p.Id);

            var orderItems = new List<OrderItem>();
            decimal totalPrice = 0;

            foreach (var item in request.Items)
            {
                var product = productDict[item.ProductId];

                if (item.Quantity <= 0)
                    throw new ArgumentException($"Số lượng không hợp lệ cho sản phẩm {product.Name}");

                var actualPrice = product.Price;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,        
                    Price = actualPrice,               
                    Size = item.Size,                  
                    Color = item.Color,                
                    Quantity = item.Quantity
                });

                totalPrice += actualPrice * item.Quantity;
            }
            var newOrder = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,  
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems
            };

            var createdOrder = await _orderRepository.CreateOrderAsync(newOrder);

            var responseDto = new OrderResponseDto
            {
                OrderId = createdOrder.Id,
                OrderDate = newOrder.OrderDate,
                TotalPrice = totalPrice,
                Items = request.Items.ToList()
            };

            return responseDto;
        }
    }
}