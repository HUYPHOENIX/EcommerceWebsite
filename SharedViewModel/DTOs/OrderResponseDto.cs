namespace SharedViewModel.DTOs;
public class OrderResponseDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemDto> Items { get; set; } = new(); 
}