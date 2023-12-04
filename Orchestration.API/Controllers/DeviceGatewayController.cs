using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestration.CertificateManagement;
using Orchestration.Commands.CommandHandlers.DeviceGateway;
using Orchestration.DeviceGatewayCommunication;
using Orchestration.DeviceGatewayCommunication.Models;
using Orchestration.Exceptions;
using Orchestration.Models;
using Orchestration.Models.DeviceGateway;
using Orchestration.Queries.QueryHandlers.DeviceGateways;
using Swashbuckle.AspNetCore.Annotations;
using Utrust360.CommunicationManager.Responses;

namespace Orchestration.API.Controllers;

/// <summary>
/// 
/// </summary>
///
[Route("general/[controller]")]
public class DeviceGatewayController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICertificate _certificate;
    private readonly IDeviceGatewayManagement _deviceGatewayManagement;
    
    
    /// <summary>
    ///  Initializes a new instance of the  <see cref="mediator"/> interface.
    ///  Initializes a new instance of the  <see cref="certificate"/> interface.
    ///  Initializes a new instance of the  <see cref="deviceGatewayManagement"/> interface.

    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="certificate"></param>
    /// <param name="deviceGatewayManagement"></param>
    public DeviceGatewayController(IMediator mediator, ICertificate certificate, IDeviceGatewayManagement deviceGatewayManagement)
    {
        _mediator = mediator;
        _certificate = certificate;
        _deviceGatewayManagement = deviceGatewayManagement;
    }
    
    
    /// <summary>
    /// Get list of device gateways
    /// </summary>
    /// <returns>List of stored devices</returns>
    /// <response code="200">Returns stored devices</response>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseResponseModel<List<DeviceGatewayViewModel>>), Description = "Return list of device gateways")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseModel<QueryException>),  Description = "Database error")]
    public async Task<BaseResponseModel<List<DeviceGatewayViewModel>>> List()
    {
        
        var relationshipsVm = await _mediator.Send(new GetDeviceGatewaysRequest());
        return new BaseResponseModel<List<DeviceGatewayViewModel>>(relationshipsVm);
    }
    
        
    /// <summary>
    /// Device gateway test connection
    /// </summary>
    /// <returns>Status of device gateway</returns>
    /// <response code="200">Returns stored devices</response>
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseResponseModel<BaseDeviceGatewayResponse>), Description = "Return device gateway status, version and build date")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(CustomException), Description = "Validation error bed request")]

    [HttpPost]
    [Route("TestConnection")]
    public async Task<BaseResponseModel<BaseDeviceGatewayResponse>> TestConnection(CreateDeviceGatewayViewModel rm)
    {
        
        var createModel = new CreateDeviceGatewayCommandModel
        {
            Name = rm.Name,
            Port = rm.Port,
            HostName = rm.HostName,
            Password = rm.Password,

        };
        
        var response = await _deviceGatewayManagement.DeviceGatewayTestConnection(createModel);
        return new BaseResponseModel<BaseDeviceGatewayResponse>(response);
    }

    
    /// <summary>
    /// Add device gateway
    /// </summary>
    /// <param name="rm">Device model</param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created,  Type = typeof(BaseResponseModel<int>), Description = " return id of newly created device gateway")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseModel<Exception>),  Description = "Internal error")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseModel<CustomException>),  Description = "Bad request")]

    public async Task<IActionResult> Create(CreateDeviceGatewayViewModel rm)
    {
     
        var certificates = await _certificate.GenerateDeviceGatewayCertificates(rm.Password, rm.HostName);

        var createModel = new CreateDeviceGatewayCommandModel
        {
            Name = rm.Name,
            Port = rm.Port,
            HostName = rm.HostName,
            Password = rm.Password,
            Certificate = Encoding.ASCII.GetBytes((string)certificates["ServerCert"])

        };

        //Test connection to DG
        await _deviceGatewayManagement.DeviceGatewayTestConnection(createModel);
        
        //Link manager to DG
        await _deviceGatewayManagement.DeviceGatewayLink(createModel, certificates);
        var deviceGateway = await _mediator.Send(createModel);
        return Created($"{deviceGateway}", new BaseResponseModel<int>() {Payload = deviceGateway } );

    }

    
    /// <summary>
    /// Delete device gateway
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:int}")]
    public async Task<BaseResponseModel<bool>> Delete(int id)
    {
        var existingDg = await _mediator.Send(new GetDeviceGatewayRequest(id));

        await _deviceGatewayManagement.DeviceGatewayUnlink(existingDg);
        var response = await _mediator.Send(new DeleteDeviceGatewayCommand(id));
        
        return  new BaseResponseModel<bool>(response > 0);
    }
}