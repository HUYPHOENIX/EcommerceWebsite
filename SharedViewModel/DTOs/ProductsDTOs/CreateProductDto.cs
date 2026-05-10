namespace SharedViewModel.DTOs;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Sizes { get; set; } = new List<string>();
    public List<string> Colors { get; set; } = new List<string>();
    public int CategoryId { get; set; }
}