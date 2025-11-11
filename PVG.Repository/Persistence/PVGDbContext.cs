using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

namespace PVG.Infrastucture.Persistence
{
    public class PVGDbContext : DbContext
    {
        public IDbConnection Connection => Database.GetDbConnection();
        public PVGDbContext(DbContextOptions<PVGDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                optionsBuilder.UseNpgsql(config.GetConnectionString("SampleConn"));

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    optionsBuilder.LogTo(Console.WriteLine);
                    optionsBuilder.EnableSensitiveDataLogging(true);
                    optionsBuilder.EnableDetailedErrors();
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entity mappings here

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        // Define DbSet properties for your entities
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}