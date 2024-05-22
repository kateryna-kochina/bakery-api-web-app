using System.ComponentModel.DataAnnotations;

namespace Bakery.Dtos;

public record CreateProductDto
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

    int CategoryId,

    [StringLength(1000)]
    string Description
);
