using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;

namespace PVG.Domain.Extensions
{
    public static class Logger
    {
        private static ILogger _logger;

        static Logger()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Đảm bảo đúng thư mục gốc
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true) // Nạp file theo môi trường (Development/Staging/...)
                .AddEnvironmentVariables()
                .Build();

            var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Logs");
            Directory.CreateDirectory(logFolder);

            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.File(
                    Path.Combine(logFolder, "SysLogs-.txt"),
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 10
                )
                .WriteTo.Console()
                .WriteTo.MySQL(
                    connectionString: config.GetConnectionString("DefaultConnection"), // Giờ sẽ không bị null nữa
                    tableName: "SysLogs",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
                )
                .CreateLogger();
        }

        public static void Error(Exception ex, string message = "Unhandled exception")
            => _logger.Error(ex, message);

        public static void Information(string message)
            => _logger.Information(message);
    }
}
