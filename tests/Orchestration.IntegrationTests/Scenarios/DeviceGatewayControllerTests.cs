using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Orchestration.API.Controllers;
using Orchestration.CertificateManagement;
using Orchestration.DataContext;
using Orchestration.DeviceGatewayCommunication;
using Orchestration.IntegrationTests.Infrastructure;
using Orchestration.Models.DeviceGateway;

namespace Orchestration.IntegrationTests.Scenarios;

public class DeviceGatewayControllerTests
{

    private static IServiceScopeFactory _scopeFactory = null!;
    private static IServiceScope _scope = null!;
    private IMediator _mediator;
    private DeviceGatewayController _controller;


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
        

        var db = scopedServices.GetRequiredService<OrchestrationDataContext>();
        db.Database.EnsureCreated();
        
    }

    [Test]
    [TestCaseSource(nameof(MockDeviceGateway))]
    public async Task can_add_update_delete_DeviceGateway(CreateDeviceGatewayViewModel model)
    {
        var result = await _controller.Create(model);
        Assert.That(201, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode!));
        var data = await _controller.List();
        Assert.Multiple(() =>
        {
            Assert.That(data.StatusCode, Is.EqualTo((ulong)HttpStatusCode.OK));
            Assert.That(data.Payload is { Count: > 0 } );
        });

        var a = data.Payload?.First();
        await _controller.Delete(a.Id);
        data = await _controller.List();
        Assert.Multiple(() =>
        {
             Assert.That(data.StatusCode, Is.EqualTo((ulong)HttpStatusCode.OK));
             Assert.That(data.Payload?.Count, Is.EqualTo(0));
        });
    }

    private static IEnumerable<TestCaseData> MockDeviceGateway
    {
        get
        {
            yield return new TestCaseData(new CreateDeviceGatewayViewModel()
            {
                
                    Name = "Neki",
                    Port = 6001,
                    Password = "123456", 
                    HostName = "172.28.4.101"
                
            }).SetName("local-DeviceGateway");
        }
    }
}