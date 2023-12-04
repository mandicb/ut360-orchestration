using Utrust360.CommunicationManager.Responses;

namespace Orchestration.DeviceGatewayCommunication;

public interface IOrchestrationPersistStorage
{
    /// <summary>
    ///  Save chsm blob to specific directory
    /// </summary>
    /// <param name="content"></param>
    /// <param name="rootPath"></param>
    /// <param name="deviceName"></param>
    /// <param name="slotNumber"></param>
    /// <returns></returns>
    Task SaveChsmBlob(Items content, string rootPath, string deviceName, int slotNumber);
}