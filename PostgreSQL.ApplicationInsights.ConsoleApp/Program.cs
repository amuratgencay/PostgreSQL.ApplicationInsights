using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostgreSQL.ApplicationInsights.ConsoleApp.Extensions;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;
using PostgreSQL.ApplicationInsights.ConsoleApp.Services;
using PostgreSQL.ApplicationInsights.Interceptor;

namespace PostgreSQL.ApplicationInsights.ConsoleApp
{
    class Program
    {
        private const string AspNetCoreEnvironmentVariable = "ASPNETCORE_ENVIRONMENT";
        private const string AppSettings = "appsettings{0}.json";
        private const string LoggingSection = "Logging";

        private static string GetAppSettings(string env = null)
            => string.Format(AppSettings, env != null ? $".{env}" : "");

        static void Main()
        {
            Console.Title = "PostgreSQL Example";
            var config = Configure();

            var serviceProvider = Setup(config);

            serviceProvider.ConfigureSeeds();

            var browser = serviceProvider.CreateInstance<Browser>();

            browser.Show();
        }

        private static ServiceProvider Setup(IConfiguration config)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection(LoggingSection));
                    loggingBuilder.AddConsole();
                })
                .AddSingleton<ICategoryService, CategoryService>()
                .AddSingleton<IProductService, ProductService>()
                .AddSingleton<IMvcService, MvcService>()
                .AddSingleton<Browser>()
                .AddViews()
                .AddApplicationInsightsPostgreSQLTelemetryServices<SalesContext>(config)
                .BuildServiceProvider();


            return serviceProvider;
        }

        private static IConfigurationRoot Configure()
        {
            var env = Environment.GetEnvironmentVariable(AspNetCoreEnvironmentVariable);
            var builder = new ConfigurationBuilder()
                .AddJsonFile(GetAppSettings(), true, true)
                .AddJsonFile(GetAppSettings(env), true, true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            return config;
        }
    }
}