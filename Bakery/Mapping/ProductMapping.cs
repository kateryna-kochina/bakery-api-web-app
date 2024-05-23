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
            product.CategoryId,
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

    public static Product ToEntity(this UpdateProductDto updatedDto, int id)
    {
        return new Product()
        {
            Id = id,
            Title = updatedDto.Title,
            Price = updatedDto.Price,
            Image = updatedDto.Image,
            CategoryId = updatedDto.CategoryId,
            Description = updatedDto.Description
        };
    }
}