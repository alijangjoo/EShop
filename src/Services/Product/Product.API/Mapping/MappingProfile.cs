using AutoMapper;
using Product.API.Entities;
using Product.API.DTOs;

namespace Product.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Entities.Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.CategoryNamePersian, opt => opt.MapFrom(src => src.Category != null ? src.Category.NamePersian : string.Empty));

        CreateMap<CreateProductDto, Entities.Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Attributes, opt => opt.Ignore());

        // Category mappings
        CreateMap<Category, CategoryDto>();

        // ProductImage mappings
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<ProductImageDto, ProductImage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        // ProductAttribute mappings
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttributeDto, ProductAttribute>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
    }
} 