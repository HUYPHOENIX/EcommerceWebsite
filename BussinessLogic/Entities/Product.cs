using System.ComponentModel.DataAnnotations;

namespace BussinessLogic.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Sizes { get; set; } = new List<string>();
    public List<string> Colors { get; set; } = new List<string>();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Foreign Key for Entity Framework
    public int CategoryId { get; set; }
    
    // Navigation property: A product belongs to one category
    public Category? Category { get; set; }
}