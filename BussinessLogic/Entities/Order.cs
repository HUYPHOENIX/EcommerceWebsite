
using System.ComponentModel.DataAnnotations;

namespace BussinessLogic.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }
        public User User {get;set;} = null!;
        public List<OrderItem> OrderItems {get; set;} = new();
    }
}