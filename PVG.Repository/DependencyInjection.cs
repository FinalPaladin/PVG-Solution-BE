using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Persistence;
using PVG.Infrastucture.Repositories.AuthTokenRepository;
using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.ImageRequestRepository;
using PVG.Infrastucture.Repositories.MDataRepository;
using PVG.Infrastucture.Repositories.NewRepository;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using PVG.Infrastucture.Repositories.ProductDetailCategoryRepository;
using PVG.Infrastucture.Repositories.ProductDetailRepository;
using PVG.Infrastucture.Repositories.ProductInfoRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.RequestCustomerDetailRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using PVG.Infrastucture.Repositories.SampleRepository;
using PVG.Infrastucture.Repositories.UserPermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using PVG.Infrastucture.Repositories.ViewLogRepository;

namespace PVG.Infrastucture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PVGDbContext>((sp, options) =>
                           options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                                            new MySqlServerVersion(new Version(8, 0, 36))));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));

            services.AddScoped<ISampleRepository, SampleRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IRequestCustomerRepository, RequestCustomerRepository>();
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IProductInfoRepository, ProductInfoRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IViewLogRepository, ViewLogRepository>();
            services.AddScoped<IImageRequestRepository, ImageRequestRepository>();
            services.AddScoped<IRequestCustomerDetailRepository, RequestCustomerDetailRepository>();
            services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
            services.AddScoped<IProductDetailCategoryRepository, ProductDetailCategoryRepository>();
            services.AddScoped<INewRepository, NewRepository>();
            services.AddScoped<IAuthTokenRepository, AuthTokenRepository>();
            services.AddScoped<IMDataRepository, MDataRepository>();

            return services;
        }
    }
}