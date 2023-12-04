using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Orchestration.API.Controllers;
using Orchestration.CertificateManagement;
using Orchestration.DataContext;
using Orchestration.DeviceGatewayCommunication;
using Orchestration.IntegrationTests.Infrastructure;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Models.DeviceGateway;

namespace Orchestration.IntegrationTests.Scenarios;

[TestFixture]
public class DeviceControllerTests
{
    private static IServiceScopeFactory _scopeFactory = null!;
    private static IServiceScope _scope = null!;
    private IMediator _mediator;
    private DeviceGatewayController _controller;
    private DeviceController _deviceController;

    

    [SetUp]
    public void Setup()
    {
        var fact = new CustomWebApplicationFactory();
        _scopeFactory = fact.Services.GetRequiredService<IServiceScopeFactory>();
        _scope = _scopeFactory.CreateScope();

        var certificate = _scope.ServiceProvider.GetRequiredService<ICertificate>();
        var deviceGatewayManagement = _scope.ServiceProvider.GetRequiredService<IDeviceGatewayManagement>();

        var scopedServices = _scope.ServiceProvider;
        _mediator = scopedServices.GetRequiredService<IMediator>();

        _controller = new DeviceGatewayController(_mediator, certificate, deviceGatewayManagement);
        _deviceController = new DeviceController(_mediator, deviceGatewayManagement);

        var db = scopedServices.GetRequiredService<OrchestrationDataContext>();
        db.Database.EnsureCreated();


        var dg = new CreateDeviceGatewayViewModel()
        {
            Name = "Neki",
            Port = 6002,
            Password = "123456",
            HostName = "192.168.0.35"
        };
        
        var result = Task.Run(() => _controller.Create(dg)).Result;

        Assert.That(201, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode!));
        
    }


    [TearDown]
    public  void BaseTearDown()
    {
        var result = Task.Run(() =>  _controller.Delete(1)).Result;
        Assert.That(result.Payload, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(MockDevice))]
    public async Task can_add_Device(CreateDeviceRequestModel model)
    {
        
        
        var data = await _deviceController.Create(model);
        Assert.That(201, Is.EqualTo(((IStatusCodeActionResult)data).StatusCode!));

        
        //check if existing credential works
        model.Credential = null;
        model.CredentialName = "TEST";
        var  deviceExistingCredential = await _deviceController.Create(model);
        Assert.That(201, Is.EqualTo(((IStatusCodeActionResult)deviceExistingCredential).StatusCode!));
        
        var deviceGatewayList = await _controller.List();
        Assert.That(deviceGatewayList.Payload != null && deviceGatewayList.Payload.Any(), Is.True);

        model.CredentialName = null;
        model.Credential = null;

        var  nocredential = await _deviceController.Create(model);



    }
    
 
    
    
    private static IEnumerable<TestCaseData> MockDevice
    {
        get
        {
            yield return new TestCaseData(new CreateDeviceRequestModel()
            {
                Name = "Test",
                HostName = "172.28.15.3",
                Port = 4000,
                Credential = new CreateCredentialRequestModel()
                {
                    Name = "TEST",
                    UserName = "admin",
                    KeyData =@"-----BEGIN EC PRIVATE KEY-----
                            MHcCAQEEIN0azH5CYTB9cChK6tspm8oodM+hXkVnzEq+ydqns1+ZoAoGCCqGSM49
                            AwEHoUQDQgAEmVX/qyFKQQaPjnEB2WEJs8tvCJkJNGL0vErZkn+EotAmENyzerNl
                            VOZ/UlwvR1+YAaqi/qdwDEYgZLuCgBGZig==
                            -----END EC PRIVATE KEY-----
                            " }
            }).SetName("device-creaplus");
        }
    }
}