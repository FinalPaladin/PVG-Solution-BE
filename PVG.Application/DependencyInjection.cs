using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using PVG.Application.Mappings;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.EmailService;
using PVG.Application.Services.PermissionService;
using PVG.Application.Services.ProductCategoryService;
using PVG.Application.Services.ProductInfoService;
using PVG.Application.Services.ProductService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Application.Services.SampleService;
using PVG.Application.Services.UserPermissionService;
using PVG.Application.Services.UserService;
using PVG.Application.Services.ViewLogService;

namespace PVG.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services here

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<ISampleService, SampleService>();
            services.AddScoped<IRequestCustomerService, RequestCustomerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductInfoService, ProductInfoService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IViewLogService, ViewLogService>();

            return services;
        }
    }
}