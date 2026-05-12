using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using SharedViewModel.DTOs;
using BusinessLogic.Mapper;

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
            var existingProducts = await _productRepository.GetProductsByIDAsync(productIds);
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
                orderItems.Add(OrderMapper.ToOrderItem(product, item));

                totalPrice += product.Price * item.Quantity;
            }
            var newOrder = OrderMapper.ToEntity(userId, totalPrice, orderItems);

            var createdOrder = await _orderRepository.CreateOrderAsync(newOrder);
            return OrderMapper.ToResponseDto(createdOrder, request.Items.ToList());
        }
    }
}