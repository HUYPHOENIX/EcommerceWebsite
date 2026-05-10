namespace SharedViewModel.DTOs;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Quantity { get; set; }
}