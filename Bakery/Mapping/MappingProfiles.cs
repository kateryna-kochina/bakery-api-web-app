using AutoMapper;
using Bakery.Dtos;
using Bakery.Models;

namespace Bakery.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // GET
        CreateMap<Category, CategoryDetailsDto>();
        CreateMap<Option, OptionDetailsDto>();
        CreateMap<Product, ProductDetailsDto>()
            .ForCtorParam(nameof(ProductDetailsDto.CategoryName),
                opt => opt.MapFrom(src => src.Category!.CategoryName));

        // POST
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<CreateOptionDto, Option>();
        CreateMap<CreateProductDto, Product>();

        // PUT
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(x => x.Id, opt => opt.Ignore());
        CreateMap<UpdateOptionDto, Option>()
            .ForMember(x => x.Id, opt => opt.Ignore());
        CreateMap<UpdateProductDto, Product>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }
}
