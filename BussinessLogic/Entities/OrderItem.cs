
namespace BussinessLogic.Entities;

public class OrderItem
{
    public int Id { get; set; } 
    public int OrderId { get; set; } 
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Quantity { get; set; }
    
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
