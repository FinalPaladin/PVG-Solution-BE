using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PVG.Infrastucture.Commons;
using PVG.Infrastucture.Entities;
using System.Data;
using System.Reflection;
using Pomelo.EntityFrameworkCore.MySql;

namespace PVG.Infrastucture.Persistence
{
    public class PVGDbContext : DbContext
    {
        public IDbConnection Connection => Database.GetDbConnection();
        public PVGDbContext(DbContextOptions<PVGDbContext> options)
            : base(options)
        {
        }

        #region Database Setting
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<ProductInfo> ProductInfo { get; set; }
        public DbSet<RequestCustomer> RequestCustomer { get; set; }
        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserPermission> UserPermission { get; set; }
        public DbSet<ViewLog> ViewLog { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                optionsBuilder.UseMySql(config.GetConnectionString("DefaultConnection"),
                                        new MySqlServerVersion(new Version(8, 0, 36)));

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

            #region Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasColumnType(ColumType.TypeVarchar("100"))
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb4")
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnType(ColumType.TypeVarchar("250"))
                    .HasMaxLength(250)
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.Image)
                    .HasColumnType(ColumType.TypeVarchar("150"))
                    .HasMaxLength(150)
                    .IsRequired();

            });
            #endregion

            #region ProductCategory
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasColumnType(ColumType.TypeVarchar("100"))
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb4")
                    .IsRequired();

            });
            #endregion

            #region ProductInfo
            modelBuilder.Entity<ProductInfo>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.ProductId)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .HasColumnType(ColumType.TypeVarchar("20"))
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnType(ColumType.TypeVarchar("250"))
                    .HasMaxLength(250)
                    .HasCharSet("utf8mb4");

                entity.Property(e => e.Content)
                    .HasColumnType(ColumType.TypeVarchar("2000"))
                    .HasMaxLength(2000)
                    .HasCharSet("utf8mb4");

            });
            #endregion

            #region RequestCustomer
            modelBuilder.Entity<RequestCustomer>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.Phone)
                    .HasColumnType(ColumType.TypeVarchar("20"))
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Key)
                    .HasColumnType(ColumType.TypeVarchar("30"))
                    .HasMaxLength(30);

                entity.Property(e => e.Value)
                    .HasColumnType(ColumType.TypeVarchar("100"))
                    .HasMaxLength(100)
                    .HasCharSet("utf8mb4");
            });
            #endregion

            #region Configuration
            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.Key)
                    .HasColumnType(ColumType.TypeVarchar("30"))
                    .HasMaxLength(30);

                entity.Property(e => e.Value)
                    .HasColumnType(ColumType.TypeVarchar("2000"))
                    .HasMaxLength(2000)
                    .HasCharSet("utf8mb4");
            });
            #endregion

            #region Permission
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasColumnType(ColumType.TypeVarchar("100"))
                    .HasMaxLength(100);
            });
            #endregion

            #region User
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.FullName)
                    .HasColumnType(ColumType.TypeVarchar("80"))
                    .HasMaxLength(80)
                    .HasCharSet("utf8mb4")
                    .IsRequired();

                entity.Property(e => e.UserName)
                    .HasColumnType(ColumType.TypeVarchar("20"))
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Password)
                    .HasColumnType(ColumType.TypeVarchar("20"))
                    .HasMaxLength(20)
                    .IsRequired();
            });
            #endregion

            #region UserPermission
            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.PermissionId)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();
            });
            #endregion

            #region ViewLog
            modelBuilder.Entity<ViewLog>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType(ColumType.TypeVarchar("36"))
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.IP)
                    .HasColumnType(ColumType.TypeVarchar("50"))
                    .HasMaxLength(50)
                    .IsRequired();
            });
            #endregion
        }
        // Define DbSet properties for your entities
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}