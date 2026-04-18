using BussinessLogic.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    // This constructor allows the API to pass in the database connection string
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // A DbSet represents a table in your database. 
    // This tells .NET: "Create a Categories table and a Products table based on my Core entities."
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
}