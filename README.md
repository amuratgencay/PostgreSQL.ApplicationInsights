# PostgreSQL.ApplicationInsights
Tracking SQL dependencies with Application Insights


## Usage Example:
```csharp
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
```
