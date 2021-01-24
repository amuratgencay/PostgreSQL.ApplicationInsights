using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Models
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SalesContext>
    {
        public SalesContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<SalesContext>();
            var connectionString = configuration.GetConnectionString("Default");
            builder.UseNpgsql(connectionString);
            return new SalesContext(builder.Options);
        }
    }
}