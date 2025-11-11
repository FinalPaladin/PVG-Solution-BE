using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using PVG.Application.Mappings;
using PVG.Application.Services.SampleService;

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

            return services;
        }
    }
}