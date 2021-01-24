using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PostgreSQL.ApplicationInsights.Interceptor
{
    public static class ApplicationInsightExtensions
    {
        private const string AppInsightsConnectionString = "ApplicationInsights:ConnectionString";
        public static IServiceCollection AddApplicationInsightsPostgreSQLTelemetryServices<T>(this IServiceCollection services,
            IConfiguration configuration) where T : DbContext
        {
            services.AddApplicationInsightsTelemetryWorkerService(
                configuration[AppInsightsConnectionString]);

            var module = CreateDatabaseDependencyTrackingTelemetryModule();

            var telemetryConfiguration = CreateTelemetryConfiguration(configuration);

            var telemetryClient = new TelemetryClient(telemetryConfiguration);
            module.Initialize(telemetryConfiguration);


            services.AddSingleton(telemetryConfiguration);
            services.AddSingleton(module);
            services.AddSingleton(telemetryClient);


            AddDbContext<T>(services, configuration, telemetryClient);
            return services;
        }

        private static void AddDbContext<T>(IServiceCollection services, IConfiguration configuration,
            TelemetryClient telemetryClient) where T : DbContext
        {
            services.AddDbContext<T>(x =>
            {
                x.AddInterceptors(new PostgreSqlInterceptor(telemetryClient));
                x.UseNpgsql(configuration.GetConnectionString("Default"));
            });
        }

        private static TelemetryConfiguration CreateTelemetryConfiguration(IConfiguration configuration)
        {
            TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.ConnectionString = configuration[AppInsightsConnectionString];
            telemetryConfiguration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());
            return telemetryConfiguration;
        }

        private static DependencyTrackingTelemetryModule CreateDatabaseDependencyTrackingTelemetryModule()
        {
            var module = new DependencyTrackingTelemetryModule();
            module.IncludeDiagnosticSourceActivities.Add("Database");
            module.EnableSqlCommandTextInstrumentation = true;
            return module;
        }
    }
}
