using Bakery.Models;
using Bakery.Dtos;

namespace Bakery.Mapping;

public static class ProductMapping
{
    public static ProductDetailsDto ToProductSummaryDto(this Product product)
    {
        return new ProductDetailsDto(
            product.Id,
            product.Title,
            product.Price,
            product.Image,
            product.Category!.CategoryName!,
            product.Description
        );
    }

    public static Product ToEntity(this CreateProductDto productDto)
    {
        return new Product()
        {
            Title = productDto.Title,
            Price = productDto.Price,
            Image = productDto.Image,
            CategoryId = productDto.CategoryId,
            Description = productDto.Description
        };
    }
}