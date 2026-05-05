namespace SharedViewModel.DTOs;
public class OrderRequestDto
{
    // TODO: We will remove UserId in the future after we got a token
    public string UserId { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}