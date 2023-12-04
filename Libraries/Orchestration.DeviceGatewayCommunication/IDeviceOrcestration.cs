using Orchestration.DeviceGatewayCommunication.Models;
using Utrust360.CommunicationManager.Responses;

namespace Orchestration.DeviceGatewayCommunication;


/// <summary>
/// Interface specific for device U.trust 360 anthor
/// </summary>
public interface IDeviceOrchestration
{
    /// <summary>
    /// Get list of slots [gladm chsm-list-slots]
    /// </summary>
    /// <param name="command">Required object</param>
    /// <param name="param"></param>
    /// <param name="receiveTimeout"></param>
    /// <param name="checkInterval"></param>
    /// <param name="connectionTimeout"></param>
    /// <returns>A task that represent get slot operation. The task result contains all used and unused slots on specific device </returns>
    Task<Items> GetSlots(DeviceCommand command, Dictionary<string, object> param, int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);
    
    
            
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command">Required object</param>
    /// <param name="param"></param>
    /// <param name="receiveTimeout"></param>
    /// <param name="checkInterval"></param>
    /// <param name="connectionTimeout"></param>
    /// <returns> </returns>
    Task<Items> GetTemplates(DeviceCommand command, Dictionary<string, object> param, int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command">Required object</param>
    /// <param name="param"></param>
    /// <param name="receiveTimeout"></param>
    /// <param name="checkInterval"></param>
    /// <param name="connectionTimeout"></param>
    /// <returns> </returns>
    Task<BaseTcpResponse> CreateChsm(DeviceCommand command, Dictionary<string, object> param, int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);
    
    
    /// <summary>
    /// Delete Chsm
    /// </summary>
    /// <param name="command">Required object</param>
    /// <param name="param"></param>
    /// <param name="receiveTimeout"></param>
    /// <param name="checkInterval"></param>
    /// <param name="connectionTimeout"></param>
    /// <returns> </returns>
    Task<BaseTcpResponse> DeleteChsm(DeviceCommand command, Dictionary<string, object> param, int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);

    /// <summary>
    /// Create Snapshot of Chsm
    /// </summary>
    /// <param name="command">Required object</param>
    /// <param name="param"></param>
    /// <param name="receiveTimeout"></param>
    /// <param name="checkInterval"></param>
    /// <param name="connectionTimeout"></param>
    /// <returns> </returns>
    Task<BaseTcpResponse> SnapshotChsm(DeviceCommand command, Dictionary<string, object> param, int? receiveTimeout = null,
        int checkInterval = 60, int? connectionTimeout = null);

}