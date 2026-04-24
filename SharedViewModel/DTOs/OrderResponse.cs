namespace SharedViewModel.DTOs;
public class OrderResponseDto
{
    //This is for after we create a Order then we want to show it to view
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemDto> Items { get; set; } = new(); 
}