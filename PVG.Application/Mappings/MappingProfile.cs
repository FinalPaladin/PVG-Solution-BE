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
            CreateMap<ProductCategory, ProductCategoryModel>().ReverseMap();
            CreateMap<ProductInfo, ProductInfoModel>().ReverseMap();
            CreateMap<New, NewModel>().ReverseMap();
            CreateMap<RequestCustomerDetail, RequestCustomerDetailModel>()
                .ForMember(des => des.CreatedDate, act => act.MapFrom(src => src.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")))
                .ReverseMap();

            CreateMap<MData, MDataModel>().ReverseMap();
            CreateMap<MData, MDataResponseModel>().ReverseMap();
        }
    }
}