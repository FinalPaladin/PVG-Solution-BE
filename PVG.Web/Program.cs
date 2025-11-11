using Serilog;
using System.Text;

namespace PVG.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Information("Starting up");
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                string type = ex.GetType().Name;
                if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

                Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
            }
            finally
            {
                Log.Information($"Shutdown complete");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddCommandLine(args)
               .Build();

            var useUrls = configuration.GetValue<string>("PVGSettings:UseUrls");

            Console.Title = typeof(Program).Assembly.GetName().Name + $" - {useUrls}";
            Console.OutputEncoding = Encoding.UTF8;

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}