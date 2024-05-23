using System.ComponentModel.DataAnnotations;

namespace Bakery.Dtos;

public record CreateCategoryDto(
    [Required]
    [StringLength(25)]
    string CategoryName
);
