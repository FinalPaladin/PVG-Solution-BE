using AutoMapper;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;

namespace PVG.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add your mapping configurations here
            // Example: CreateMap<SourceModel, DestinationModel>();
            // You can also use ReverseMap() for bi-directional mapping

            CreateMap<ConfigurationModel, Configuration>().ReverseMap();
            CreateMap<Configuration, ConfigurationModel>().ReverseMap();
            CreateMap<RequestCustomerModel, RequestCustomer>().ReverseMap();
            CreateMap<RequestCustomer, RequestCustomerModel>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<Permission, PermissionModel>().ReverseMap();
            CreateMap<UserPermission, UserPermissionModel>().ReverseMap();
            CreateMap<Product, ProductModel>().ReverseMap();
            CreateMap<Product, ProductResponseModel>().ReverseMap();
            CreateMap<Product, ProductCreateRequest>().ReverseMap();
            CreateMap<Product, ProductUpdateRequest>().ReverseMap();
            CreateMap<ProductCategory, ProductCategoryModel>().ReverseMap();
            CreateMap<RequestCustomerDetail, RequestCustomerDetailModel>()
                .ForMember(des => des.CreatedDate, act => act.MapFrom(src => src.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")))
                .ReverseMap();

            CreateMap<MData, MDataModel>().ReverseMap();
            CreateMap<MData, MDataResponseModel>().ReverseMap();

            CreateMap<ProductDetail, ProductDetailModel>().ReverseMap();
            CreateMap<ProductDetail, ProductDetailResponseModel>().ReverseMap();

            CreateMap<ProductUpdateRequest, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.CreatedByName, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore());

            CreateMap<ProductDetailUpdateModel, ProductDetail>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ProductId, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.CreatedByName, opt => opt.Ignore())
                .ForMember(x => x.CreatedDate, opt => opt.Ignore());
        }
    }
}