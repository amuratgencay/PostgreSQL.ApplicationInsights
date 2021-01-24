using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Models
{
    public class SalesContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(
                    "User ID=postgres;Password=123;Host=localhost;Port=5432;Database=sales;Pooling=false;");
        }

        protected SalesContext()
        {
        }

        public SalesContext(DbContextOptions options) : base(options)
        {
        }
    }
}