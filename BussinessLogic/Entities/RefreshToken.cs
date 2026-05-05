using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BussinessLogic.Entities;

public class RefreshToken
{
    [Key]
    public int Id {get;set;}
    public string Token { get; set; } = string.Empty; 
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    // This can be added or not this foreign key syntax, but this time i will add it just for learning purpose .
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}