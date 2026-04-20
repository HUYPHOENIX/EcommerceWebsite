using System.ComponentModel.DataAnnotations.Schema;

namespace SharedViewModel.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Sizes { get; set; } = new List<string>();
        public List<string> Colors { get; set; } = new List<string>();

        // We keep the CategoryId so we know what category it belongs to, 
        // but we do NOT include the actual Category object here.
        public int CategoryId { get; set; }
    }
}