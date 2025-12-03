using PVG.Domain.Settings;

namespace PVG.Web.Extensions
{
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddConfigureAppSetting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"),
                o => o.BindNonPublicProperties = true);

            return services;
        }
    }
}
