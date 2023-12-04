using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orchestration.API;
using Orchestration.API.Middleware;
using Orchestration.CertificateManagement;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.DeviceGatewayCommunication;
using Utrust360.Crypto.Services;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.IntegrationTests.Infrastructure;

public class TestStartup : Startup
{
    public TestStartup(IConfiguration configuration) : base(configuration)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        
        var cnn = new SqliteConnection("Filename=:memory:");
        cnn.Open();
        
        services.AddDbContext<OrchestrationDataContext>(options =>
        {
            options.UseSqlite(cnn);
        });

        services.RegisterCommandHandlers();
        services.AddSingleton<ICryptoService, CryptoService>();

        services.AddSingleton<ICertificate, Certificate>();
        services.AddSingleton<IDeviceGatewayManagement, DeviceGatewayManagement>();
        services.AddSingleton<IDeviceOrchestration, DeviceOrchestration>();


    }
    
    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    { 
        app.UseControllerExceptionHandlingMiddleware();
    }
}