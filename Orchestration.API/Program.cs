using Microsoft.EntityFrameworkCore;
using Orchestration.API;
using Orchestration.DataContext;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

// builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
// {
//     loggerConfiguration.WriteTo.File("Logs/OperationService-.txt", rollingInterval: RollingInterval.Day, shared: true);
//     loggerConfiguration.WriteTo.Console();
// });

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(o =>
    {
        if (o is HttpContext ctx)
        {
            // In order to get relative location headers (without the host part), we modify any location header here
            // This is to simplify the reverse-proxy setup in front of the application
            try
            {
                if (!string.IsNullOrEmpty(context.Response.Headers.Location))
                {
                    var locationUrl = new Uri(context.Response.Headers.Location);
                    context.Response.Headers.Location = locationUrl.PathAndQuery;
                }
            }
            catch (Exception) { }
        }
        return Task.CompletedTask;
    }, context);
    await next();
});

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<OrchestrationDataContext>();
    context.Database.EnsureCreated();
}


startup.Configure(app, app.Environment);
app.Run();

