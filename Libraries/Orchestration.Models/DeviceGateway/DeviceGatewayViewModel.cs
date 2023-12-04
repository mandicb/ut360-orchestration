using System.Text.Json.Serialization;
using Orchestration.Models.Device;
using Orchestration.Models.Device.ViewModels;

namespace Orchestration.Models.DeviceGateway;

/// <summary>
///  Device gateways response model
/// </summary>
public class DeviceGatewayViewModel
{
    /// <example>1</example>
    public int Id { get; set; } 
    
    /// <example>deviceGateway1</example>
    public string Name { get; set; } = null!;
    
    /// <example>172.1.1.1</example>
    public string HostName { get; set; } = null!;
    
    /// <example>6001</example>
    public int Port { get; set; }
   
    /// <example>12345</example>
    [JsonIgnore]
    public string Password { get; init; } = null!;
    
    /// <example>Certificate data</example>
    [JsonIgnore]
    public byte[] Certificate { get; set; } = null!;
    
}