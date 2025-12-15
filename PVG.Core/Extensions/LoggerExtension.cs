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
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var logFolder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "App_Data",
                "Logs"
            );

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
                    connectionString: config.GetConnectionString("DefaultConnection"),
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
