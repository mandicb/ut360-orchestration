using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;

// Setup Serilog logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");
    var wao = new WebApplicationOptions {
        Args = args
    };
    
    var builder = WebApplication.CreateBuilder(wao);
    
    
    // Use Serilog logging for ASP.NET Core
    builder.Host.UseSerilog();

    

    // Use Startup with the new minimal hosting model
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    // Use Autofac IoC container
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(startup.ConfigureContainer);

    var app = builder.Build();

    startup.Configure(app, app.Environment);
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
