using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PVG.Application.Mappings;
using PVG.Application.Services.CloudflareR2Service;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.EmailService;
using PVG.Application.Services.InitPageService;
using PVG.Application.Services.MDataService;
using PVG.Application.Services.NewService;
using PVG.Application.Services.PermissionService;
using PVG.Application.Services.ProductCategoryService;
using PVG.Application.Services.ProductInfoService;
using PVG.Application.Services.ProductService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Application.Services.SampleService;
using PVG.Application.Services.TokenService;
using PVG.Application.Services.UserPermissionService;
using PVG.Application.Services.UserService;
using PVG.Application.Services.ViewLogService;
using PVG.Infrastucture.Entities;

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

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

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
            services.AddScoped<INewService, NewService>();
            services.AddScoped<IInitPageService, InitPageService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICloudflareR2Service, CloudflareR2Service>();
            services.AddScoped<IMDataService, MDataService>();

            return services;
        }
    }
}