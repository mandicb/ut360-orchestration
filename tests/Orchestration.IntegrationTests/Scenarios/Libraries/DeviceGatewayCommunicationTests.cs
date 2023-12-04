using System.Text;
using Administration.Infrastructure.Model;
using Microsoft.Extensions.DependencyInjection;

using Orchestration.CertificateManagement;
using Orchestration.DeviceGatewayCommunication;
using Orchestration.DeviceGatewayCommunication.Models;
using Orchestration.IntegrationTests.Infrastructure;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Models.DeviceGateway;
using Utrust360.Crypto.Model;
using Utrust360.Crypto.Services.Abstraction;
using Utrust360.DAL.Enums;

namespace Orchestration.IntegrationTests.Scenarios.Libraries;


[TestFixture]
public class DeviceGatewayCommunicationTests
{
    
    private ICertificate _certificate = null!;
    private IDeviceGatewayManagement _deviceGatewayManagement = null!;
    private IDeviceOrchestration _deviceOrchestration = null!;

    private ICryptoService? _cryptoService;
    
    [SetUp]
    public void Setup()
    {
    
        
        var fact = new CustomWebApplicationFactory();
        var scopeFactory = fact.Services.GetRequiredService<IServiceScopeFactory>(); 
        var scope = scopeFactory.CreateScope();
        _certificate = scope.ServiceProvider.GetRequiredService<ICertificate>();
        _deviceGatewayManagement = scope.ServiceProvider.GetRequiredService<IDeviceGatewayManagement>();
        _deviceOrchestration = scope.ServiceProvider.GetRequiredService<IDeviceOrchestration>();
        _cryptoService = scope.ServiceProvider.GetRequiredService<ICryptoService>();
        
        Environment.SetEnvironmentVariable("IP", "192.168.1.1");
        
        
    }

    [Test]
    [TestCaseSource(nameof(MockDeviceGateway))]
    public  async Task can_link_unlink_Device_Gateway(DeviceCommand model, CreateDeviceGatewayCommandModel db)
    {
        var r =  await _certificate.GenerateDeviceGatewayCertificates("123456", model.DeviceGateway.HostName);
        
        var parameters = new Dictionary<string, object>()
        {
            { "CACert",  r["CACert"] },
            { "ServerCert", r["ServerCert"] },
            { "ServerCertKey", r["ServerCertKey"] }
        };
        
        Assert.DoesNotThrowAsync(async () =>  await _deviceGatewayManagement.DeviceGatewayLink(db, parameters));

        model.DeviceGateway.Certificate =  Encoding.ASCII.GetBytes((string)r["ServerCert"]);
        
        var deviceViewModel = new DeviceGatewayViewModel()
        {
            Certificate = model.DeviceGateway.Certificate,
            HostName = model.DeviceGateway.HostName,
            Name = model.DeviceGateway.Name,
            Password = model.DeviceGateway.Password,
            Port = model.DeviceGateway.Port
        };
        
        await _deviceGatewayManagement.DeviceGatewayUnlink(deviceViewModel);
    }

    [Test]
    [TestCaseSource(nameof(MockDeviceGateway))]
    public  async Task can_Add_Test_Device_Connection(DeviceCommand model, CreateDeviceGatewayCommandModel dg)
    {
        var parameters = new Dictionary<string, object>()
        {
            { "Content",  TestConnectionCommand.Content.info_group.ToString() },
        };

        
        var r =  await _certificate.GenerateDeviceGatewayCertificates("123456", model.DeviceGateway.HostName);
        
        var parameters2 = new Dictionary<string, object>()
        {
            { "CACert",  r["CACert"] },
            { "ServerCert", r["ServerCert"] },
            { "ServerCertKey", r["ServerCertKey"] }
        };
        
        await _deviceGatewayManagement.DeviceGatewayLink(dg, parameters2);
        

        model.DeviceGateway.Certificate = Encoding.ASCII.GetBytes((string)r["ServerCert"]);
        model.Device.Credential.KeyData = _cryptoService.Encrypt(model.Device.Credential.KeyData,
            await _cryptoService.GetKeyByType(ProtectionKeys.KeyType.General));
        
       var deviceGatewayConnection =  await _deviceGatewayManagement.DeviceGatewayTestConnection(dg, null);
        
       Assert.That(deviceGatewayConnection.Version, !Is.Null);
       
       var deviceTestConnection = await _deviceGatewayManagement.DeviceTestConnection(model, parameters);
       Assert.That(deviceTestConnection.HardwareVersion, !Is.Null);

       model.Device.Id = new Random().Next(10);

       
       //Add devicee
       var a = await _deviceGatewayManagement.AddDeleteDevice(model, Machine.MachineStatus.Online);
       
       Assert.That(a.Valid, Is.True);
        
       //Delete device
       var deleteDevice = await _deviceGatewayManagement.AddDeleteDevice(model, Machine.MachineStatus.Deleted);
       model.DeviceGateway.Certificate =  Encoding.ASCII.GetBytes((string)r["ServerCert"]);

       var deviceViewModel = new DeviceGatewayViewModel()
       {
           Certificate = model.DeviceGateway.Certificate,
           HostName = model.DeviceGateway.HostName,
           Name = model.DeviceGateway.Name,
           Password = model.DeviceGateway.Password,
           Port = model.DeviceGateway.Port
       };
       await _deviceGatewayManagement.DeviceGatewayUnlink(deviceViewModel);
    }

    [Test, Description("HSM Orchestration Get slot list")]
    [TestCaseSource(nameof(MockDeviceGateway))]
    public async Task can_get_Slot_List(DeviceCommand model, CreateDeviceGatewayCommandModel dg)
    {
        var parameters = new Dictionary<string, object>()
        {
            { "Content",  TestConnectionCommand.Content.info_group.ToString() },
        };

        
        var r =  await _certificate.GenerateDeviceGatewayCertificates("123456", model.DeviceGateway.HostName);
        
        var parameters2 = new Dictionary<string, object>()
        {
            { "CACert",  r["CACert"] },
            { "ServerCert", r["ServerCert"] },
            { "ServerCertKey", r["ServerCertKey"] }
        };
        
        await _deviceGatewayManagement.DeviceGatewayLink(dg, parameters2);
        
        
        model.DeviceGateway.Certificate = Encoding.ASCII.GetBytes((string)r["ServerCert"]);
        model.Device.Credential.KeyData = _cryptoService.Encrypt(model.Device.Credential.KeyData, await _cryptoService.GetKeyByType(ProtectionKeys.KeyType.General));
        model.Device.Id = new Random().Next(100000);

        await _deviceGatewayManagement.AddDeleteDevice(model, Machine.MachineStatus.Online);

        var parameterssd = new Dictionary<string, object> { { "MachineId", model.Device.Id }, { "Timeout", 20  } };

        var response = await _deviceOrchestration.GetSlots(model, parameterssd);
        
        Assert.That(response.Valid, Is.True);

        var deviceViewModel = new DeviceGatewayViewModel()
        {
            Certificate = model.DeviceGateway.Certificate,
            HostName = model.DeviceGateway.HostName,
            Name = model.DeviceGateway.Name,
            Password = model.DeviceGateway.Password,
            Port = model.DeviceGateway.Port
        };
        await _deviceGatewayManagement.DeviceGatewayUnlink(deviceViewModel);

    }
    
    [Test, Description("HSM Orchestration Add chsm")]
    [TestCaseSource(nameof(MockDeviceGateway))]
    public async Task can_add_chsm(DeviceCommand model, CreateDeviceGatewayCommandModel dg)
    {
        var r =  await _certificate.GenerateDeviceGatewayCertificates("123456", model.DeviceGateway.HostName);
        
        var parameters2 = new Dictionary<string, object>()
        {
            { "CACert",  r["CACert"] },
            { "ServerCert", r["ServerCert"] },
            { "ServerCertKey", r["ServerCertKey"] }
        };
        
        await _deviceGatewayManagement.DeviceGatewayLink(dg, parameters2);
        
        
        model.DeviceGateway.Certificate = Encoding.ASCII.GetBytes((string)r["ServerCert"]);
        model.Device.Credential.KeyData = _cryptoService.Encrypt(model.Device.Credential.KeyData, await _cryptoService.GetKeyByType(ProtectionKeys.KeyType.General));
        model.Device.Id = new Random().Next(100000);

        await _deviceGatewayManagement.AddDeleteDevice(model, Machine.MachineStatus.Online);

        var paramGetSlot = new Dictionary<string, object> { { "MachineId", model.Device.Id }, { "Timeout", 20  } };
        
        await _deviceOrchestration.GetSlots(model, paramGetSlot);
        var parameterAdd = new Dictionary<string, object> { 
            { "MachineId", model.Device.Id }, 
            { "Slot", "16" },
            { "Timeout", 20  }
        };


        var parameterChsmDelete = new Dictionary<string, object>
        {
            { "MachineId", model.Device.Id },
            { "Slot", new []{16} },

        };
        // var response = await _deviceOrchestration.Create(model, parameterAdd);
        
        // Assert.That(response.Valid, Is.True);

       var adr = await _deviceOrchestration.DeleteChsm(model, parameterChsmDelete);

        var slotListAfterDelete = await _deviceOrchestration.GetSlots(model, paramGetSlot);
        
        
        var deviceViewModel = new DeviceGatewayViewModel()
        {
            Certificate = model.DeviceGateway.Certificate,
            HostName = model.DeviceGateway.HostName,
            Name = model.DeviceGateway.Name,
            Password = model.DeviceGateway.Password,
            Port = model.DeviceGateway.Port
        };
        await _deviceGatewayManagement.DeviceGatewayUnlink(deviceViewModel);

    }
    
     
    [Test, Description("HSM Orchestration Device template list")]
    [TestCaseSource(nameof(MockDeviceGateway))]
    public async Task can_get_template_List(DeviceCommand model, CreateDeviceGatewayCommandModel dg)
    {
        var r =  await _certificate.GenerateDeviceGatewayCertificates("123456", model.DeviceGateway.HostName);
        
        var parameters2 = new Dictionary<string, object>()
        {
            { "CACert",  r["CACert"] },
            { "ServerCert", r["ServerCert"] },
            { "ServerCertKey", r["ServerCertKey"] }
        };
        
        await _deviceGatewayManagement.DeviceGatewayLink(dg, parameters2);
        
        
        model.DeviceGateway.Certificate = Encoding.ASCII.GetBytes((string)r["ServerCert"]);
        model.Device.Credential.KeyData = _cryptoService.Encrypt(model.Device.Credential.KeyData, await _cryptoService.GetKeyByType(ProtectionKeys.KeyType.General));
        model.Device.Id = new Random().Next(100000);

        await _deviceGatewayManagement.AddDeleteDevice(model, Machine.MachineStatus.Online);

        var paramGetSlot = new Dictionary<string, object> { { "MachineId", model.Device.Id }, { "Timeout", 20  } };
        var templates = await _deviceOrchestration.GetTemplates(model, paramGetSlot);
      
        
        Assert.That(templates.Valid, Is.True);

        var deviceViewModel = new DeviceGatewayViewModel()
        {
            Certificate = model.DeviceGateway.Certificate,
            HostName = model.DeviceGateway.HostName,
            Name = model.DeviceGateway.Name,
            Password = model.DeviceGateway.Password,
            Port = model.DeviceGateway.Port
        };
        await _deviceGatewayManagement.DeviceGatewayUnlink(deviceViewModel);
    }
    

    private static IEnumerable<TestCaseData> MockDeviceGateway
    {
        get
        {
            yield return new TestCaseData(
                
                new DeviceCommand()
                {
                    DeviceGateway =  new DeviceGatewayViewModel()
                    {
                        Name = "Neki",
                        Port = 6001,
                        Password = "123456",
                        HostName = "172.28.4.101"
                    },
                    Device = new DeviceViewModel()
                    {
                        Name = "Test",
                        HostName = "172.31.3.79",
                        Port = 4000,
                        Credential = new CreateCredentialRequestModel()
                        {
                            Name = "TEST",
                            UserName = "admin",
                            KeyData = @"-----BEGIN EC PRIVATE KEY-----
                            MHcCAQEEIN0azH5CYTB9cChK6tspm8oodM+hXkVnzEq+ydqns1+ZoAoGCCqGSM49
                            AwEHoUQDQgAEmVX/qyFKQQaPjnEB2WEJs8tvCJkJNGL0vErZkn+EotAmENyzerNl
                            VOZ/UlwvR1+YAaqi/qdwDEYgZLuCgBGZig==
                            -----END EC PRIVATE KEY-----"
                            // KeyData = "-----BEGIN EC PRIVATE KEY-----\nMIHcAgEBBEIB61o75z0asSf6cC07VinuQnKxgW6W8lTDdxOEJ8gy9pe2UQ4p9teg\njVOQfqxGIGoaLn6enWW5j/To2NeyQlCQDQigBwYFK4EEACOhgYkDgYYABAH+xJly\ngDA8pjwcrIn+Qw/6uzav6WNXGgQH4g61x4yXMewEa1/swe2WI/YdHTVg9B9z9rS2\nhFzAwN3V5eKWm6t/8AB2Ej5hb/bz4ELy6rAXA/9ajdTmKv/ubY+5qY5cHgMXLlEt\nmkEOxc7ui67rEnpdU2kXLsmF7jeaTd+ZqR8r35p+pg==\n-----END EC PRIVATE KEY-----"
                        }
                        
                    }
                }, new CreateDeviceGatewayCommandModel()
                {
                    
                    Name = "Neki",
                    Port = 6001,
                    Password = "123456",
                    HostName = "172.28.4.101"
                }
          ).SetName("deviceGateway-1");
            
        }
    }
    
}