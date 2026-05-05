namespace SharedViewModel.DTOs;
public class OrderResponseDto
{
    // TODO: Notice: We will remove UserId in the future after we got a token
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemDto> Items { get; set; } = new(); 
}