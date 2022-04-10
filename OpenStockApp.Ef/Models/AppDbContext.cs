using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenStockApi.Core.Models.Discord;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Ef.Models
{
    public class AppDbContext : DbContext
    {
        private readonly string _dbConnectionString;
        public AppDbContext(IOptions<DatabaseConfiguration> options)
        {
            if(string.IsNullOrEmpty(options.Value.ConnectionString))
                throw new ArgumentNullException(nameof(options.Value.ConnectionString));
            _dbConnectionString = options.Value.ConnectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(_dbConnectionString);
        }

        public DbSet<Model> Models => Set<Model>();
        public DbSet<Sku> Skus => Set<Sku>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Category> Categories => Set<Category>();

        //regional
        public DbSet<Continent> Continents => Set<Continent>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<Region> Regions => Set<Region>();

        public DbSet<Retailer> Retailers => Set<Retailer>();


        //Discord
        public DbSet<DiscordRole> DiscordRoles => Set<DiscordRole>();
        public DbSet<DiscordChannel> DiscordChannels => Set<DiscordChannel>();

        

    }
    
}
