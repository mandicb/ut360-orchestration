using Administration.Infrastructure.Model;
using Orchestration.DeviceGatewayCommunication.Models;
using Orchestration.Models.DeviceGateway;
using Utrust360.CommunicationManager.Responses;


namespace Orchestration.DeviceGatewayCommunication;


//TODO: ADD comments regarding param Optionals
public interface IDeviceGatewayManagement
{
    
    /// <summary>
    /// Link  davicegateway to manager (SET-CERT)
    /// </summary>
    /// <param name="model"></param>
    /// <param name="parameters"></param>
    /// <param name="receiveTimeout">Optional </param>
    /// <param name="checkInterval">Optional </param>
    /// <param name="connectionTimeout">Optional</param>
    /// <returns></returns>
    Task DeviceGatewayLink(CreateDeviceGatewayCommandModel model,  Dictionary<string, object> parameters, int? connectionTimeout = null, int? receiveTimeout = 30000, int checkInterval = 60);
    

    /// <summary>
    /// Unlink devicegateway from manager
    /// </summary>
    /// <param name="model"></param>
    /// <param name="receiveTimeout"></param>
    /// <param name="checkInterval"></param>
    /// <param name="connectionTimeout"></param>
    /// <returns></returns>
    Task DeviceGatewayUnlink(DeviceGatewayViewModel model, int? receiveTimeout = 30000, int checkInterval = 60,  int? connectionTimeout = null);


    
    /// <summary>
    ///  Device gateway test connection
    /// </summary>
    /// <param name="model"></param>
    /// <param name="receiveTimeout">Optional </param>
    /// <param name="checkInterval">Optional </param>
    /// <param name="connectionTimeout">Optional</param>
    /// <returns>A task that represents test connection. The task result contains the device gateway response (status, version, build date) </returns>
    Task<BaseDeviceGatewayResponse> DeviceGatewayTestConnection(CreateDeviceGatewayCommandModel model, int? receiveTimeout = 30000,
        int checkInterval = 60, int? connectionTimeout = null);
    
    /// <summary>
    /// Device test connection
    /// </summary>
    /// <param name="command"></param>
    /// <param name="parameters"></param>
    /// <param name="receiveTimeout">Optional </param>
    /// <param name="checkInterval">Optional </param>
    /// <param name="connectionTimeout">Optional</param>
    /// <returns>A task that represents test connection. The task result contains the device gateway response (version, hardwareversion, ) </returns>
    Task<BaseDeviceResponse> DeviceTestConnection(DeviceCommand command, Dictionary<string, object>? parameters = null , int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="receiveTimeout"></param>
    /// <param name="checkInterval"></param>
    /// <param name="connectionTimeout"></param>
    /// <returns></returns>
    Task<BaseTcpResponse> GetDevices(DeviceGatewayViewModel command, int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);
    
    
    /// <summary>
    /// Add or delete device
    /// </summary>
    /// <param name="command">Request object</param>
    /// <param name="status">Device status (Online, offline)</param>
    /// <param name="receiveTimeout">Optional </param>
    /// <param name="checkInterval">Optional </param>
    /// <param name="connectionTimeout">Optional</param>
    /// <returns>A task that represents adding or delete device operation. The task result contains the device gateway response </returns>
    Task<BaseTcpResponse> AddDeleteDevice(DeviceCommand command, Machine.MachineStatus status, int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);
}