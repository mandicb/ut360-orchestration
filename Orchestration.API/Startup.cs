
using System.Reflection;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Orchestration.API.Middleware;
using Orchestration.CertificateManagement;
using Orchestration.Commands.infrastructure;
using Orchestration.DataContext;
using Orchestration.DeviceGatewayCommunication;
using Orchestration.Queries.Infrastructure;
using Utrust360.Crypto.Services;
using Utrust360.Crypto.Services.Abstraction;

namespace Orchestration.API;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public virtual void ConfigureServices(IServiceCollection services)
    {
        
        
        
        services.AddHttpContextAccessor();
        
        //TODO: FIX connection string for production add password
        var cnn = new SqliteConnection(_configuration.GetConnectionString("ModelConnection"));
        services.AddDbContext<OrchestrationDataContext>( x =>
            x.UseSqlite(cnn)
        );

        
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (!string.IsNullOrEmpty(environment) && (environment.Equals("Development") || environment.Equals("Dev")))
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true));
            });
        }
        
        else
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed(host => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }
        
        
        services.AddControllers();
        
        services.AddOpenApiDocument(document =>
        {
            document.Title = "HSM Orchestration service";
            document.Description = "Orchestration service";
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(x =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            x.IncludeXmlComments(xmlPath);
        });
        services.AddFluentValidationRulesToSwagger();



        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        // Add services to the container.
        services.RegisterCommandHandlers();
        services.RegisterQueryHandlers();
        
        
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<ICertificate, Certificate>();
        services.AddSingleton<IDeviceGatewayManagement, DeviceGatewayManagement>();
        services.AddSingleton<IDeviceOrchestration, DeviceOrchestration>();


    }


   

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var pathBase = _configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment() || env.IsStaging() || (env != null && env.EnvironmentName.Equals("Dev")))
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi3(settings =>
            {
                
            });
        }

        app.UseControllerExceptionHandlingMiddleware();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireCors("CorsPolicy");
        });
    }
}
