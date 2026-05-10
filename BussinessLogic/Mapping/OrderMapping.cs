using BussinessLogic.Entities;
using SharedViewModel.DTOs;

namespace BusinessLogic.Mapper
{
    public static class OrderMapper
    {
        public static OrderItem ToOrderItem(
            Product product,
            OrderItemDto itemDto)
        {
            return new OrderItem
            {
                ProductId = itemDto.ProductId,
                ProductName = product.Name,
                Price = product.Price,
                Size = itemDto.Size?.Trim() ?? string.Empty,
                Color = itemDto.Color?.Trim() ?? string.Empty,
                Quantity = itemDto.Quantity
            };
        }

        public static Order ToEntity(
            string userId,
            decimal totalPrice,
            List<OrderItem> orderItems)
        {
            return new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems
            };
        }

        public static OrderResponseDto ToResponseDto(
            Order order,
            List<OrderItemDto> items)
        {
            return new OrderResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Items = items
            };
        }
    }
}