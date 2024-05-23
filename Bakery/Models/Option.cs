namespace Bakery.Models;

public class Option
{
    public int Id { get; set; }
    public required string OptionName { get; set; }
    public required double Coefficient { get; set; }
}
