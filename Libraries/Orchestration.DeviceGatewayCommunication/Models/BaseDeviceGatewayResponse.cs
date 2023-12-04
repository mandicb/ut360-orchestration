namespace Orchestration.DeviceGatewayCommunication.Models;

public class BaseDeviceGatewayResponse
{
    
    ///<example>5.0</example> 
    public string? Version { get; set; }
    
    ///<example>Active</example> 
    public string? Status { get; set; }
    
    
    ///<example>Oct 11 2023 17:24:05</example> 
    public string? BuildDate { get; set; }
    
}