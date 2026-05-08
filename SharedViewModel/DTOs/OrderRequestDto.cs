using System.Reflection.Metadata;

namespace SharedViewModel.DTOs;
public class OrderRequestDto
{
    public List<OrderItemDto> Items { get; set; } = new();
}