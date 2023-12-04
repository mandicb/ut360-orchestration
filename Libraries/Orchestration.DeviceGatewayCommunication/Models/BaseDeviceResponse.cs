namespace Orchestration.DeviceGatewayCommunication.Models;

public class BaseDeviceResponse
{
    /// <example>4.5.0</example>
    public string Version { get; set; } = null!;
    
    /// <example>CS13214</example>
    public string SerialNumber { get; set; } = null!;
    /// <example>7.03.0.13</example>
    public string HardwareVersion { get; set; } = null!;
}