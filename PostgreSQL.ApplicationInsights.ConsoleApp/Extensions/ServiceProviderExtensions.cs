using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PostgreSQL.ApplicationInsights.ConsoleApp.Migrations.Seeds;
using PostgreSQL.ApplicationInsights.ConsoleApp.Models;
using PostgreSQL.ApplicationInsights.ConsoleApp.Views;

namespace PostgreSQL.ApplicationInsights.ConsoleApp.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void ConfigureSeeds(this ServiceProvider serviceProvider)
        {
            var salesContext = serviceProvider.GetService<SalesContext>();
            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterface(nameof(ISeeder)) != null)
                .Select(t => (ISeeder) Activator.CreateInstance(t, salesContext))
                .ToList()
                .ForEach(x => x.Seed());
        }

        public static IServiceCollection AddViews(this IServiceCollection serviceCollection)
        {
            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterface(nameof(IView)) != null)
                .ToList().ForEach(x => { serviceCollection.AddSingleton(x); });
            return serviceCollection;
        }

        public static T CreateInstance<T>(this ServiceProvider serviceProvider)
            => ActivatorUtilities.CreateInstance<T>(serviceProvider);

        public static object CreateInstance(this ServiceProvider serviceProvider, Type instanceType,
            params object[] parameters)
            => ActivatorUtilities.CreateInstance(serviceProvider, instanceType, parameters);
    }
}