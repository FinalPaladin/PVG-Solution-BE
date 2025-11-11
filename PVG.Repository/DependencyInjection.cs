using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Persistence;
using PVG.Infrastucture.Repositories.SampleReporitory;
using Microsoft.EntityFrameworkCore;

namespace PVG.Infrastucture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PVGDbContext>((sp, options) =>
                           options.UseNpgsql(configuration.GetConnectionString("SampleConn"))
                           //.AddInterceptors(sp.GetRequiredService<AutoFactorInterceptor>())
                           );
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));

            services.AddTransient<ISampleRepository, SampleRepository>();

            return services;
        }
    }
}