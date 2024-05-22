using System.ComponentModel.DataAnnotations;

namespace Bakery.Dtos;

public record ProductDetailsDto(
    int Id,
    string Title,
    decimal Price,
    string Image,
    string CategoryName,
    string? Description
);
