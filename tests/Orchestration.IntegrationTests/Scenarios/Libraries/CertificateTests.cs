using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Orchestration.CertificateManagement;
using Orchestration.IntegrationTests.Infrastructure;

namespace Orchestration.IntegrationTests.Scenarios.Libraries;

[TestFixture]
public class CertificateTests
{
    
    
    public Certificate _certificate; 
    
    
    [SetUp]
    public void Setup()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Identity", "https://localhost" },
            { "Administration", "https://localhost" },
            { "DefaultCAPath", $"./cert/utrust360CA.pfx" },
            { "ASPNETCORE_Kestrel:Certificates:Default:Password", "t8DCssPFyaxFRVvBQeHumg==" }
        };

        var logger = new Mock<ILogger<Certificate>>();
        
        var fact = new CustomWebApplicationFactory();
        var scopeFactory = fact.Services.GetRequiredService<IServiceScopeFactory>(); 
        var scope = scopeFactory.CreateScope();
        
        
        Environment.SetEnvironmentVariable("IP", "192.168.1.1");
        
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        _certificate = new Certificate(configuration,  logger.Object);
    }

    [Test]
    public  async Task can_generate_device_gateway_certificate()
    {
        var r =  await _certificate.GenerateDeviceGatewayCertificates("123456", "creaplus.com");
        
        byte[] bytes = Encoding.ASCII.GetBytes((string)r["ServerCert"]);
        
        Assert.DoesNotThrow(() =>   new X509Certificate2(bytes, "123456JIE)sUhjdjn09"));
        
    }
}