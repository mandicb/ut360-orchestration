using Administration.Infrastructure.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestration.Commands.CommandHandlers.Devices;
using Orchestration.DeviceGatewayCommunication;
using Orchestration.DeviceGatewayCommunication.Models;
using Orchestration.Exceptions;
using Orchestration.Models;
using Orchestration.Models.Credential;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;
using Orchestration.Queries.QueryHandlers.Credential;
using Orchestration.Queries.QueryHandlers.DeviceGateways;
using Orchestration.Queries.QueryHandlers.Devices;
using Swashbuckle.AspNetCore.Annotations;


//TODO: add 401 response swagger!!
namespace Orchestration.API.Controllers;

/// <summary>
/// Device  controller
/// </summary>
///
[Route("general/[controller]")]

public class DeviceController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IDeviceGatewayManagement _deviceGatewayManagement;

    /// <summary>
    ///  Initializes a new instance of the  <see cref="mediator"/> interface.
    ///  Initializes a new instance of the  <see cref="deviceGatewayManagement"/> interface.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="deviceGatewayManagement"></param>
    public DeviceController(IMediator mediator, IDeviceGatewayManagement deviceGatewayManagement)
    {
        _mediator = mediator;
        _deviceGatewayManagement = deviceGatewayManagement;
    }
    
    /// <summary>
    /// List of devices
    /// </summary>
    /// <returns>List of stored devices</returns>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseResponseModel<List<DeviceListModel>>), Description = "Return list of devices")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseModel<QueryException>),  Description = "Database error")]
    public async Task<BaseResponseModel<List<DeviceListModel>>> List()
    {
        var devices = await _mediator.Send(new GetDevicesRequest());
        return new BaseResponseModel<List<DeviceListModel>>(devices);
    }
    
    /// <summary>
    /// Device gateway test connection
    /// </summary>
    /// <returns>Status of device gateway</returns>
    /// <response code="200">Returns stored devices</response>
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseResponseModel<BaseDeviceResponse>), Description = "Return device status")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(CustomException), Description = "Validation error bed request")]
    [HttpPost]
    [Route("TestConnection")]
    public async Task<BaseResponseModel<BaseDeviceResponse>> TestConnection(CreateDeviceRequestModel rm)
    {
        var credential = rm.Credential ?? await _mediator.Send(new GetCredentialRequest(rm.CredentialName, true));

        var deviceGateway = await _mediator.Send(new GetDeviceGatewayRequest(rm.DeviceGatewayId));
        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = deviceGateway,
            Device = new DeviceViewModel()
            {
                Credential = credential,
                HostName = rm.HostName,
                Port = rm.Port,
                Name = rm.Name,
                Id = rm.Id
            }
        };
        
        await _deviceGatewayManagement.DeviceTestConnection(deviceCommand);
        
        var response = await _deviceGatewayManagement.DeviceTestConnection(deviceCommand);
        return new BaseResponseModel<BaseDeviceResponse>(response);
    }
    
    /// <summary>
    /// Create device
    /// </summary>
    /// <param name="rm">Device model</param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    /// <exception cref="Exception"></exception>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(BaseResponseModel<int>), Description = "Device successfully added return id of device")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseModel<CustomException>), Description = "Bad request")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseModel<Exception>), Description = "Internal server error")]
    public async Task<IActionResult> Create(CreateDeviceRequestModel rm)
    {
        
        if (rm.Credential == null && string.IsNullOrEmpty(rm.CredentialName))
        {
            throw new CustomException("Pleas provide credential name or credential data");
        }

        var credential = rm.Credential ?? await _mediator.Send(new GetCredentialRequest(rm.CredentialName, true));
        
        var deviceGateway = await _mediator.Send(new GetDeviceGatewayRequest(rm.DeviceGatewayId));
        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = deviceGateway,
            Device = new DeviceViewModel()
            {
                Credential = credential,
                HostName = rm.HostName,
                Port = rm.Port,
                Name = rm.Name,
                Id = rm.Id
            }
        };
        
        await _deviceGatewayManagement.DeviceTestConnection(deviceCommand);

        var data = await _mediator.Send(new CreateDeviceCommandModel()
        {
            Name = rm.Name,
            Port = rm.Port,
            HostName = rm.HostName,
            DeviceGatewayId = rm.DeviceGatewayId,
            CredentialName = rm.CredentialName,
            Credential =  new CreatCredentialCommandModel()
            {
                Id = credential.Id,
                Name = credential.Name,
                KeyData = credential.KeyData,
                KeyPassword = credential.KeyPassword,
                UserName = credential.UserName
            } 
        });
        
        deviceCommand.Device.Id = data;
        
        var dataAddDevice = await _deviceGatewayManagement.AddDeleteDevice(deviceCommand, Machine.MachineStatus.Online);
        if (dataAddDevice.Valid)
        {
            return Created($"{data}", new BaseResponseModel<int>(payload: data));
        }
        await _mediator.Send(new DeleteDeviceCommand(rm.Id));
        throw new CustomException("Adding device to device gateway failed");

    }
    
    /// <summary>
    /// Delete device
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{deviceId}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int), Description = "Return number of deleted devices")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseModel<CustomException>))]
    public async Task<BaseResponseModel<int>> Delete(int deviceId)
    {
        
        //TODO: CHECK IF DEVICE IS DELETED FROM DG!!
        
        var existingDevice = await _mediator.Send(new GetDeviceRequest(deviceId, true));
        
        //// Check if device is not exists
        //// return 404

        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = existingDevice.DeviceGateway,
            Device = existingDevice
        };
        
        await _deviceGatewayManagement.AddDeleteDevice(deviceCommand, Machine.MachineStatus.Offline); 
        var data = await _mediator.Send(new DeleteDeviceCommand(deviceId));
       
       return new BaseResponseModel<int>(data);
    }
}
