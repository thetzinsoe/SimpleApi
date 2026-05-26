using AutoMapper;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}