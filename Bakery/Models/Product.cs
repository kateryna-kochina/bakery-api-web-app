namespace Bakery.Models;

public class Product
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public required string Image { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? Description { get; set; }
}
