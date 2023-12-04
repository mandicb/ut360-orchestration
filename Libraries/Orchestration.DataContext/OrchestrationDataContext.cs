using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Orchestration.DataContext.Models;

namespace Orchestration.DataContext;

public class OrchestrationDataContext : DbContext
{
    
    public OrchestrationDataContext(DbContextOptions<OrchestrationDataContext> options)
        : base(options)
    {
    }

    /*
       * 
       * TO ADD MIGRATION RUN:
        dotnet ef migrations add initMigration -c .\Libraries\Orchestration.DataContext\OrchestrationDataContext.cs -p .\Libraries\Orchestration.DataContext\  -s .\Orchestration.API       
        dotnet ef migrations add initMigration -c OrchestrationDataContext inside Orchestration.DataContext* 
       * TO REMOVE MIGRATION RUN:
       * Remove-Migration -Project RDA.DataContext -StartUpProject RDA.DataContext -Context RemoteDeviceAttestationDataContext
       * 
       * TO MANUAL UPDATE DB
        * dotnet ef database update  -p .\Libraries\RDA.DataContext -c RemoteDeviceAttestationDataContext -s .\Libraries\RDA.DataContext
       * Update-Database -Project RDA.DataContext -StartUpProject RDA.DataContext -Context RemoteDeviceAttestationDataContext
    */
    
    #region DbSets

    public DbSet<DeviceGateway> DeviceGateways { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Credential> Credentials { get; set; }

    #endregion
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Credential>(x =>
        {
            x.HasIndex(c => c.Name)
                .IsUnique();
        });
        
        
 
        #region Indexes

        modelBuilder.Entity<DeviceGateway>()
            .HasIndex(i => new { i.Name, i.HostName, i.Port });

        modelBuilder.Entity<Device>()
            .HasIndex(i => i.Name); 

        modelBuilder.Entity<Credential>()
            .HasIndex(i => i.Name);

        #endregion Indexes
    }
    
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrchestrationDataContext>
    {
        public OrchestrationDataContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot configuration;
            if(string.IsNullOrWhiteSpace(env) || env.Equals("Development"))
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile(@Directory.GetCurrentDirectory() + $"/appsettings.json")
                    .Build();
            }
            else
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile(@Directory.GetCurrentDirectory() + $"/appsettings.{env}.json")
                    .Build();
            }

            var builder = new DbContextOptionsBuilder<OrchestrationDataContext>()
                .UseSqlite(configuration.GetConnectionString("ModelConnection"));

            return new OrchestrationDataContext(builder.Options);
        }
    }
    
}