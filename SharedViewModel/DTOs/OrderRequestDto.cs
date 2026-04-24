namespace SharedViewModel.DTOs;
public class OrderRequestDto
{
    //Notice: Order Request here need to know whose order we are create for
    public string UserId { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}