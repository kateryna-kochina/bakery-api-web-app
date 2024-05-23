using System.ComponentModel.DataAnnotations;

namespace Bakery.Dtos;

public record UpdateProductDto
(
    [Required]
    [StringLength(50)]
    string Title,

    [Required]
    [Range(0,1000)]
    decimal Price,

    [Required]
    [StringLength(500)]
    string Image,

    [Required]
    int CategoryId,

    [StringLength(1000)]
    string? Description
);
