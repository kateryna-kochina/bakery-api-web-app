namespace Bakery.Dtos;

public record ProductDetailsDto(
    int Id,
    string Title,
    decimal Price,
    string Image,
    int CategoryId,
    string CategoryName,
    string? Description
);
