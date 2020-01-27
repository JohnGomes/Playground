using System;
using System.Diagnostics;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using EntityConfigurations;
    using Model;
    using Microsoft.EntityFrameworkCore.Design;

    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
        }     
    }


    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<CatalogContext>
    {
        public CatalogContext CreateDbContext(string[] args)
        {
            
            // // Build config
            // IConfiguration config = new ConfigurationBuilder()
            //     .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../EfDesignDemo"))
            //     .AddJsonFile("appsettings.json")
            //     .Build();
            //
            // // Get connection string
            // var optionsBuilder = new DbContextOptionsBuilder<ProductsDbContext>();
            // var connectionString = config.GetConnectionString(nameof(ProductsDbContext));
            // optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("EfDesignDemo.EF.Design"));
            // return new ProductsDbContext(optionsBuilder.Options);
            //
            //
            
            var connectionString =
                "server=localhost,1434;user id=sa;password=Pass@word123;database=Microsoft.eShopOnContainers.Services.CatalogDb;";
            var optionsBuilder =  new DbContextOptionsBuilder<CatalogContext>()
                .UseSqlServer(connectionString);
            
            Console.WriteLine(connectionString);

            return new CatalogContext(optionsBuilder.Options);
        }
    }
}
