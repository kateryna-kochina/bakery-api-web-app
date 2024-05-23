using System.ComponentModel.DataAnnotations;

namespace Bakery.Dtos;

public record UpdateCategoryDto(
    [Required]
    [StringLength(25)]
    string CategoryName
);
