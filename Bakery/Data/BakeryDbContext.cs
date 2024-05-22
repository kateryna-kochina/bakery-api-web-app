using Bakery.Models;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Data;

public class BakeryDbContext : DbContext
{
    public BakeryDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Option> Options { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .Entity<Category>()
            .HasData(
                new { Id = 1, CategoryName = "Cakes" },
                new { Id = 2, CategoryName = "Cheesecakes" },
                new { Id = 3, CategoryName = "Muffins" },
                new { Id = 4, CategoryName = "Pies" }
        );

        builder
            .Entity<Option>()
            .HasData(
                new { Id = 1, OptionName = "Slice", Coefficient = 0.125 },
                new { Id = 2, OptionName = "Half", Coefficient = 0.5 },
                new { Id = 3, OptionName = "Whole", Coefficient = 1.0 },
                new { Id = 4, OptionName = "One", Coefficient = 1.0 },
                new { Id = 5, OptionName = "Six", Coefficient = 6.0 },
                new { Id = 6, OptionName = "Dozen", Coefficient = 12.0 }
        );
    }
}
