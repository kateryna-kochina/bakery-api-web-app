using System.ComponentModel.DataAnnotations;

namespace Bakery.Dtos;

public record UpdateOptionDto(
    [Required]
    [StringLength(25)]
    string OptionName,

    [Required]
    double Coefficient
);
