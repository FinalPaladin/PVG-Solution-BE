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

            CreateMap<ConfigurationModel, Configuration>();
            CreateMap<Configuration, ConfigurationModel>();
            CreateMap<RequestCustomerModel, RequestCustomer>();
            CreateMap<RequestCustomer, RequestCustomerModel>();
            CreateMap<User, UserModel>();
            CreateMap<Permission, PermissionModel>();
            CreateMap<UserPermission, UserPermissionModel>();
            CreateMap<Product, ProductModel>();
            CreateMap<ProductCategory, ProductCategoryModel>();
            CreateMap<ProductInfo, ProductInfoModel>();
            CreateMap<New, NewModel>();
        }
    }
}