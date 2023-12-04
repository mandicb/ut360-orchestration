using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestration.API;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.Queries.Infrastructure;

namespace Orchestration.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
                builder.UseStartup<TestStartup>());
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var path = Assembly.GetAssembly(typeof(CustomWebApplicationFactory))!
                .Location;

            var directoryName = Path.GetDirectoryName(path) ?? string.Empty;
            builder.UseContentRoot(directoryName)
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables();
                });

            _ = new ConfigurationBuilder()
                .SetBasePath(directoryName)
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            _ = builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();
                services.RegisterCommandHandlers();
                services.RegisterQueryHandlers();
            });
        }
}
