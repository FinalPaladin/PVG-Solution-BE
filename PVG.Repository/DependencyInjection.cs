using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Persistence;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.SampleRepository;

namespace PVG.Infrastucture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PVGDbContext>((sp, options) =>
                           options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                                            new MySqlServerVersion(new Version(8, 0, 36))));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));

            services.AddTransient<ISampleRepository, SampleRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();

            return services;
        }

    }
}