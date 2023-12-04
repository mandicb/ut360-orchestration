using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestration.DeviceGatewayCommunication;
using Orchestration.DeviceGatewayCommunication.Models;
using Orchestration.Models;
using Orchestration.Models.Device;
using Orchestration.Models.Orchestration;
using Orchestration.Queries.QueryHandlers.Devices;
using Utrust360.CommunicationManager.Responses;

namespace Orchestration.API.Controllers;

/// <summary>
///  Orchestration controller
/// </summary>
[Route("management/[controller]")]
public class OrchestrationController : BaseController
{
   
    private readonly IMediator _mediator;
    private readonly IDeviceOrchestration _deviceOrchestration;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="deviceGatewayManagement"></param>
    public OrchestrationController(IMediator mediator, IDeviceOrchestration deviceGatewayManagement)
    {
        _mediator = mediator;
        _deviceOrchestration = deviceGatewayManagement;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Route("{deviceId}/Slots")]
    [HttpGet]
    public async Task<BaseResponseModel<Items>> GetSlots(int deviceId)
    {
        var existingDevice = await _mediator.Send(new GetDeviceRequest(deviceId, true));
        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = existingDevice.DeviceGateway,
            Device = existingDevice
        };
        
        var param = new Dictionary<string, object> { { "MachineId", deviceId }, { "Timeout", 20  } };
        var result = await _deviceOrchestration.GetSlots(deviceCommand, param);
        
        return new BaseResponseModel<Items>(result);
    }
    
    
    /// <summary>
    /// Get list of templates
    /// </summary>
    /// <returns></returns>
    [Route("{deviceId}/Templates")]
    [HttpGet]
    public async Task<BaseResponseModel<Items>> GetTemplates(int deviceId)
    {
        var existingDevice = await _mediator.Send(new GetDeviceRequest(deviceId, true));
        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = existingDevice.DeviceGateway,
            Device = existingDevice
        };
        
        var param = new Dictionary<string, object> { { "MachineId", deviceId }, { "Timeout", 20  } };
        var result = await _deviceOrchestration.GetTemplates(deviceCommand, param);
        
        return new BaseResponseModel<Items>(result);
    }
    
     
    /// <summary>
    /// Create chsm
    /// </summary>
    /// <returns></returns>
    [Route("{deviceId}/Create")]
    [HttpPost]
    public async Task<BaseResponseModel<List<Items>>> Create(int deviceId, [FromBody]CreateChsmCommandModel model)
    {
        var existingDevice = await _mediator.Send(new GetDeviceRequest(deviceId, true));
        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = existingDevice.DeviceGateway,
            Device = existingDevice
        };
        
        var param = new Dictionary<string, object> { { "MachineId", deviceId }, { "Slot", model.Slot.ToString()  } };
        
        var result = await _deviceOrchestration.CreateChsm(deviceCommand, param);
        
        return new BaseResponseModel<List<Items>>(result.Results);
    }
    
    /// <summary>
    /// Snapshot chsm
    /// </summary>
    /// <returns></returns>
    [Route("{deviceId}/Snapshot/{slotId}")]
    [HttpPost]
    public async Task<BaseResponseModel<List<Items>>> Snapshot(int deviceId, int slotId)
    {
        var existingDevice = await _mediator.Send(new GetDeviceRequest(deviceId, true));
        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = existingDevice.DeviceGateway,
            Device = existingDevice
        };
        
        var pamsSnapshot = new Dictionary<string, object> { 
            { "MachineId", deviceId },
            { "Slot", slotId.ToString() }, 
            { "Full", 1 }
        };

        
        
        var result = await _deviceOrchestration.SnapshotChsm(deviceCommand, pamsSnapshot);
        
        return new BaseResponseModel<List<Items>>(result.Results);
    }

    /// <summary>
    /// Delete chsm
    /// </summary>
    /// <returns></returns>
    [Route("{deviceId}/Delete")]
    [HttpDelete]
    public async Task<BaseResponseModel<List<Items>>> Delete(int deviceId, [FromBody]int[] model)
    {
        var existingDevice = await _mediator.Send(new GetDeviceRequest(deviceId, true));
        var deviceCommand = new DeviceCommand()
        {
            DeviceGateway = existingDevice.DeviceGateway,
            Device = existingDevice
        };
        
        var param = new Dictionary<string, object> { { "MachineId", deviceId }, { "Slot", model  } };
        var result = await _deviceOrchestration.DeleteChsm(deviceCommand, param);
        
        return new BaseResponseModel<List<Items>>(result.Results);
    }
}