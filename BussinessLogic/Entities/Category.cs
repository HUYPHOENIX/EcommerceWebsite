using System.ComponentModel.DataAnnotations;

namespace BussinessLogic.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation property: A category can have many products
    public ICollection<Product> Products { get; set; } = new List<Product>();
}